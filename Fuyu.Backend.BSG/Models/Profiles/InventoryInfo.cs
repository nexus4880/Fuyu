using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Hideout;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Services;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class InventoryInfo
{
    public Dictionary<MongoId, ItemInstance> ItemsMap { get; private set; } = [];

    public List<ItemInstance> Items => ItemsMap.Values.ToList();

    [DataMember(Name = "items")]
    private List<ItemInstance> _itemsForSerialization;

    [OnSerializing]
    private void OnSerializing(StreamingContext _)
    {
        _itemsForSerialization = Items;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext _)
    {

        ItemsMap = _itemsForSerialization.ToDictionary(i => i.Id);

        /*var itemFactoryService = ItemFactoryService.Instance;
        var itemService = ItemService.Instance;
        foreach (var item in Items)
        {
            var props = itemFactoryService.GetItemProperties<CompoundItemItemProperties>(item.TemplateId);

            if (props.Grids.Count > 0)
            {
                var items = itemService.GetItemAndChildren(Items, item);

                try
                {
                    item.InitializeMatrices(props.Grids, items);
                }
                catch (Exception ex)
                {
                    // this fails on bot gifter (and maybe others)
                    Terminal.WriteLine($"Failed to initialize matrices: {ex}");
                }
            }
        }*/
    }

    [DataMember(Name = "equipment")]
    public MongoId Equipment { get; set; }

    [DataMember(Name = "stash")]
    public MongoId? Stash { get; set; }

    [DataMember(Name = "questRaidItems")]
    public MongoId? QuestRaidItems { get; set; }

    [DataMember(Name = "questStashItems")]
    public MongoId? QuestStashItems { get; set; }

    [DataMember(Name = "sortingTable")]
    public MongoId? SortingTable { get; set; }

    [DataMember(Name = "hideoutAreaStashes")]
    public Dictionary<EAreaType, MongoId> HideoutAreaStashes { get; set; }

    [DataMember(Name = "fastPanel")]
    // TODO: proper type later
    public Dictionary<string, MongoId> FastPanel { get; set; }

    [DataMember(Name = "favoriteItems")]
    public List<MongoId> FavoriteItems { get; set; }

    [DataMember(Name = "hideoutCustomizationStashId")]
    public MongoId? HideoutCustomizationStashId { get; set; }

    public async Task<(LocationInGrid, string)> GetNextFreeSlotAsync(ItemService itemService, int width, int height,
        EItemRotation desiredRotation = EItemRotation.Horizontal)
    {
        var stashMatrix = await StashItem.Matrices.GetMatrixAsync(itemService, "hideout");
        var result = await itemService.GetNextFreeSlotAsync(StashItem, Items, width, height, stashMatrix, desiredRotation);
        return result;
    }

    public async Task AddItems(ItemService itemService, List<ItemInstance> itemStack)
    {
        if (itemStack.Count == 0)
        {
            throw new Exception($"{nameof(itemStack)}.Count must be greater than 0");
        }

        var rootItem = itemStack[0];

        if (!rootItem.Location.IsValue1)
        {
            throw new Exception("!rootItem.Location.IsValue1");
        }

        if (rootItem.Location.Value1 == null)
        {
            throw new Exception("Location is null");
        }

        var stash = StashItem;

        if (stash == null)
        {
            throw new Exception($"Failed to find stash");
        }

        var matrix = await stash.Matrices.GetMatrixAsync(itemService, rootItem.SlotId);
        if (matrix is null)
        {
            throw new Exception("Matrix not initialized");
        }

        (int width, int height) = await itemService.CalculateItemSizeAsync(itemStack, rootItem.Location.Value1.r);
        var x = rootItem.Location.Value1.x;
        var y = rootItem.Location.Value1.y;

        for (var dy = 0; dy < height; dy++)
        {
            for (var dx = 0; dx < width; dx++)
            {
                var tempX = x + dx;
                var tempY = y + dy;

                if (matrix[tempX, tempY])
                {
                    throw new Exception("Overlap");
                }

                matrix[tempX, tempY] = true;
            }
        }

        foreach (var item in itemStack)
        {
            ItemsMap[item.Id] = item;
        }
    }

    /// <returns>The root item, not the full stack</returns>
    public ItemInstance FindItem(MongoId id)
    {
        if (!ItemsMap.TryGetValue(id, out var item))
        {
            item = null;
        }

        return item;
    }

    public async Task MoveItemAsync(ItemService itemService, ItemFactoryService itemFactoryService, List<ItemInstance> items, MongoId parentItemId, string targetSlot, LocationInGrid targetLocation)
    {
        var rootItem = items[0];
        var previousOwnerItem = FindItem(rootItem.ParentId);

        // If it is a subitem such as ammo/attachment there
        // is no matrix to update and this won't get hit
        if (rootItem.Location.IsValue1 && rootItem.Location.Value1 != null)
        {
            var previousLocation = rootItem.Location.Value1;
            (int rootItemWidth, int rootItemHeight) = await itemService.CalculateItemSizeAsync(items, rootItem.Location.Value1.r);

            var matrix = await previousOwnerItem.Matrices.GetMatrixAsync(itemService, rootItem.SlotId);
            if (matrix is not null)
            {
                for (var dy = 0; dy < rootItemHeight; dy++)
                {
                    for (var dx = 0; dx < rootItemWidth; dx++)
                    {
                        var x = previousLocation.x + dx;
                        var y = previousLocation.y + dy;

                        // Mark the previous slots as no longer occupied
                        matrix[x, y] = false;
                    }
                }
            }
        }

        if (targetLocation != null)
        {
            var targetItem = FindItem(parentItemId);
            rootItem.Size = null;

            // Recalculate with the target rotation in mind
            (int rootItemWidth2, int rootItemHeight2) = await itemService.CalculateItemSizeAsync(items, targetLocation.r);
            if (targetItem.Matrices is null)
            {
                var props = await itemFactoryService.GetItemPropertiesAsync<CompoundItemItemProperties>(targetItem.TemplateId);
                if (props.Grids.Count > 0)
                {
                    targetItem.InitializeMatrices(props.Grids, items);
                }
            }

            var targetMatrix = await targetItem.Matrices.GetMatrixAsync(itemService, targetSlot);
            if (targetMatrix is not null)
            {
                for (var dy = 0; dy < rootItemHeight2; dy++)
                {
                    for (var dx = 0; dx < rootItemWidth2; dx++)
                    {
                        var x = targetLocation.x + dx;
                        var y = targetLocation.y + dy;

                        if (targetMatrix[x, y])
                        {
                            throw new Exception("Overlap");
                        }

                        // Mark the new slots as occupied
                        targetMatrix[x, y] = true;
                    }
                }
            }
        }

        rootItem.Location = targetLocation;
        rootItem.ParentId = parentItemId;
        rootItem.SlotId = targetSlot;
    }

    public async Task<List<ItemInstance>> RemoveItemAsync(ItemService itemService, ItemInstance rootItem)
    {
        if (rootItem == null)
        {
            throw new ArgumentNullException(nameof(rootItem));
        }

        if (!ItemsMap.ContainsKey(rootItem.Id))
        {
            throw new Exception($"{rootItem.Id} does not exist in InventoryInfo");
        }

        var containerItem = ItemsMap[rootItem.ParentId];

        if (containerItem == null)
        {
            throw new Exception($"Failed to find container {rootItem.ParentId} for {rootItem.Id}");
        }

        var itemAndChildren = itemService.GetItemAndChildren(Items, rootItem);

        /// We only need to update the <see cref="ItemInstance.Matrices"/> if the
        /// <see cref="ItemInstance.Location"/> is <see cref="LocationInGrid"/>
        if (rootItem.Location.IsValue1 && rootItem.Location.Value1 != null)
        {
            var matrix = await containerItem.Matrices.GetMatrixAsync(itemService, rootItem.SlotId);
            if (matrix is null)
            {
                throw new Exception("Matrix not initialized");
            }

            (int width, int height) = await itemService.CalculateItemSizeAsync(itemAndChildren, rootItem.Location.Value1.r);
            var previousX = rootItem.Location.Value1.x;
            var previousY = rootItem.Location.Value1.y;

            for (var dy = 0; dy < height; dy++)
            {
                for (var dx = 0; dx < width; dx++)
                {
                    var tempPreviousX = previousX + dx;
                    var tempPreviousY = previousY + dy;

                    matrix[tempPreviousX, tempPreviousY] = false;
                }
            }
        }

        foreach (var item in itemAndChildren)
        {
            ItemsMap.Remove(item.Id);
        }

        return itemAndChildren;
    }

    /// <returns>The item and children that were removed</returns>
    public Task<List<ItemInstance>> RemoveItemAsync(ItemService itemService, MongoId id)
    {
        return RemoveItemAsync(itemService, ItemsMap[id]);
    }

    public List<ItemInstance> GetItemAndChildren(ItemService itemService, MongoId id)
    {
        return itemService.GetItemAndChildren(Items, id);
    }

    public List<ItemInstance> GetItemAndChildren(ItemService itemService, ItemInstance rootItem)
    {
        return itemService.GetItemAndChildren(Items, rootItem);
    }

    public List<ItemInstance> GetItemsByTemplate(MongoId templateId)
    {
        return Items.FindAll(i => i.TemplateId == templateId);
    }

    public ItemInstance StashItem
    {
        get
        {
            if (Stash.HasValue && ItemsMap.TryGetValue(Stash.Value, out var itemInstance))
            {
                return itemInstance;
            }

            return null;
        }
    }
}