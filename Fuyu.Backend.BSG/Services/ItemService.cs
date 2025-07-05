using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Services;

public class ItemService
{
    private readonly ItemFactoryService _itemFactoryService;
    private readonly IItemTemplateRepository _itemTemplates;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public ItemService(ItemFactoryService itemFactoryService, IItemTemplateRepository itemTemplates)
    {
        _itemFactoryService = itemFactoryService;
        _itemTemplates = itemTemplates;
    }

    public static void RegenerateItemIds(IEnumerable<ItemInstance> items, Dictionary<string, string> mapping)
    {
        // replace ids
        foreach (var item in items)
        {
            // replace item id
            item.Id = mapping[item.Id];

            // replace item's parent id
            if (item.ParentId != null && mapping.TryGetValue(item.ParentId, out var parentId))
            {
                item.ParentId = parentId;
            }
        }
    }

    public static void RegenerateItemIds(List<ItemInstance> items)
    {
        var mapping = new Dictionary<string, string>();
        foreach (var item in items)
        {
            mapping.TryAdd(item.Id, MongoId.Generate());
        }

        RegenerateItemIds(items, mapping);
    }

    public List<ItemInstance> GetItemAndChildren(List<ItemInstance> items, ItemInstance item)
    {
        return GetItemAndChildren(items, item.Id);
    }

    public bool IsChildItem(ItemInstance item, List<MongoId> ids)
    {
        if (item.ParentId == null)
        {
            return false;
        }

        return ids.FindIndex(i => i == item.ParentId) != -1;
    }

    public List<ItemInstance> GetItemAndChildren(List<ItemInstance> items, MongoId id)
    {
        var rootItem = items.Find(i => i.Id == id);
        var result = new List<ItemInstance>() { rootItem };
        var subItems = items.FindAll(i => i.ParentId == rootItem.Id);

        foreach (var item in subItems)
        {
            var recurseResult = GetItemAndChildren(items, item.Id);

            result.AddRange(recurseResult);
        }

        return result;
    }

    /// <summary>
    /// Only an item and its children should be passed into this
    /// </summary>
    public async Task<(int width, int height)> CalculateItemSizeAsync(List<ItemInstance> items, EItemRotation rotation)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (items.Count == 0)
        {
            throw new Exception($"{nameof(items)} is empty");
        }

        var root = items[0];
        var isFolded = root.Updatable?.Foldable?.Folded == true;

        if (root.Size == null)
        {
            var rootProperties = await _itemFactoryService.GetItemPropertiesAsync<CompoundItemItemProperties>(root.TemplateId);

            var width = rootProperties.Width;
            var height = rootProperties.Height;

            var sizeUp = 0;
            var sizeDown = 0;
            var sizeLeft = 0;
            var sizeRight = 0;
            var forcedUp = 0;
            var forcedDown = 0;
            var forcedLeft = 0;
            var forcedRight = 0;

            // For items with grids (backpack/stash/containers) we should not add onto the size
            if (rootProperties.Grids.Count == 0)
            {
                for (var i = 1; i < items.Count; i++)
                {
                    var itemProperties = await _itemFactoryService.GetItemPropertiesAsync<ItemProperties>(items[i].TemplateId);

                    if (itemProperties == null)
                    {
                        continue;
                    }

                    if (isFolded && items[i].SlotId == "mod_stock")
                    {
                        continue;
                    }

                    if (itemProperties.ExtraSizeForceAdd)
                    {
                        forcedUp += itemProperties.ExtraSizeUp;
                        forcedDown += itemProperties.ExtraSizeDown;
                        forcedLeft += itemProperties.ExtraSizeLeft;
                        forcedRight += itemProperties.ExtraSizeRight;
                    }
                    else
                    {
                        sizeUp = Math.Max(sizeUp, itemProperties.ExtraSizeUp);
                        sizeDown = Math.Max(sizeDown, itemProperties.ExtraSizeDown);
                        sizeLeft = Math.Max(sizeLeft, itemProperties.ExtraSizeLeft);
                        sizeRight = Math.Max(sizeRight, itemProperties.ExtraSizeRight);
                    }
                }
            }

            width += sizeLeft + sizeRight + forcedLeft + forcedRight;
            height += sizeUp + sizeDown + forcedUp + forcedDown;

            root.Size = (width, height);
        }

        // If the desired rotation is vertical then flip
        // the height and the width
        if (rotation == EItemRotation.Vertical)
        {
            (int w, int h) = root.Size.Value;

            return (h, w);
        }

        return root.Size.Value;
    }

    public async Task<(LocationInGrid, string)> GetNextFreeSlotAsync(ItemInstance containerItem,
        List<ItemInstance> items, int width, int height, bool[,] matrix, EItemRotation desiredRotation = EItemRotation.Horizontal)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        string gridName = null;

        var containerItemProperties =
            await _itemFactoryService.GetItemPropertiesAsync<CompoundItemItemProperties>(containerItem.TemplateId);

        if (containerItemProperties?.Grids == null)
        {
            return (null, null);
        }

        foreach (var grid in containerItemProperties.Grids)
        {
            gridName = grid.Name;
            var gridWidth = grid.Properties.CellsHorizontal;
            var gridHeight = grid.Properties.CellsVertical;

            // Check if the item is too big for this grid
            if (width > gridWidth || height > gridHeight)
            {
                continue;
            }

            if (matrix == null)
            {
                matrix = await GenerateMatrixAsync(gridWidth, gridHeight, items);
            }

            for (var y = 0; y <= gridHeight - height; y++)
            {
                for (var x = 0; x <= gridWidth - width; x++)
                {
                    var canFit = true;

                    for (var dy = 0; canFit && dy < height; dy++)
                    {
                        for (var dx = 0; dx < width; dx++)
                        {
                            if (matrix[x + dx, y + dy])
                            {
                                canFit = false;
                                break;
                            }
                        }
                    }

                    if (canFit)
                    {
                        return (new LocationInGrid { x = x, y = y, r = desiredRotation }, gridName);
                    }
                }
            }
        }

        return (null, null);
    }

    public async Task<bool[,]> GenerateMatrixAsync(int gridWidth, int gridHeight, List<ItemInstance> items)
    {
        var matrix = new bool[gridWidth, gridHeight];

        foreach (var itemInThisGrid in items)
        {
            if (!itemInThisGrid.Location.IsValue1 || itemInThisGrid.Location.Value1 == null)
            {
                continue;
            }

            var itemLocation = itemInThisGrid.Location.Value1;
            var itemAndChildren = GetItemAndChildren(items, itemInThisGrid);
            (int itemWidth, int itemHeight) = await CalculateItemSizeAsync(itemAndChildren, itemLocation.r);

            if (itemLocation.x < 0 || itemLocation.y < 0 ||
                itemLocation.x + itemWidth > gridWidth ||
                itemLocation.y + itemHeight > gridHeight)
            {
                continue;
            }

            for (var y = 0; y < itemHeight; y++)
            {
                for (var x = 0; x < itemWidth; x++)
                {
                    var cellX = itemLocation.x + x;
                    var cellY = itemLocation.y + y;
                    matrix[cellX, cellY] = true;
                }
            }
        }

        return matrix;
    }

    public async Task<bool> IsFunctionalAsync(List<ItemInstance> items, ItemInstance rootItem)
    {
        if (items.Count == 0)
        {
            throw new Exception($"{nameof(items)}.Count == 0");
        }

        if (rootItem == null)
        {
            throw new ArgumentNullException(nameof(rootItem));
        }

        var rootItemTemplate = await _itemTemplates.GetItemTemplateAsync(rootItem.TemplateId);

        if (rootItemTemplate == null)
        {
            throw new Exception($"Failed to find ItemTemplate for {rootItem.TemplateId}");
        }

        var itemProperties = _itemFactoryService.GetItemProperties<CompoundItemItemProperties>(rootItemTemplate);
        var weaponItemProperties = _itemFactoryService.GetItemProperties<WeaponItemProperties>(rootItemTemplate);
        var result = new List<Offer>();

        if (itemProperties.Slots != null)
        {
            foreach (var slot in itemProperties.Slots)
            {
                if (!slot.Required)
                {
                    continue;
                }

                if (!items.Exists(i => i.SlotId == slot.Name && i.ParentId == rootItem.Id))
                {
                    return false;
                }
            }
        }

        if (weaponItemProperties.Chambers != null)
        {
            foreach (var chamber in weaponItemProperties.Chambers)
            {
                if (!chamber.Required)
                {
                    continue;
                }

                if (!items.Exists(i => rootItem.SlotId == chamber.Name && i.ParentId == rootItem.Id))
                {
                    return false;
                }
            }
        }

        return true;
    }
}