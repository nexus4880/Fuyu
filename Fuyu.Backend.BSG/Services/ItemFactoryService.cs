using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Common.Hashing;
using Fuyu.Common.Serialization;
using Newtonsoft.Json;

namespace Fuyu.Backend.BSG.Services;

// TODO: split UPD factory and Item Factory
public class ItemFactoryService
{
    private readonly IItemTemplateRepository _itemTemplates;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public ItemFactoryService(IItemTemplateRepository itemTemplates)
    {
        _itemTemplates = itemTemplates;
    }

    /// <summary>
    /// Gets an <see cref="ItemProperties"/> from a <see cref="MongoId"/> TemplateId
    /// </summary>
    /// <typeparam name="T">The <see cref="ItemProperties"/> class to return</typeparam>
    /// <param name="templateId">The <see cref="MongoId"/> TemplateId to get the <see cref="ItemProperties"/> from</param>
    /// <returns>The <see cref="ItemProperties"/> class defined</returns>
    public async Task<T> GetItemPropertiesAsync<T>(MongoId templateId) where T : ItemProperties
    {
        var itemTemplate = await _itemTemplates.GetItemTemplateAsync(templateId);

        return GetItemProperties<T>(itemTemplate);
    }

    /// <summary>
    /// Gets an <see cref="ItemProperties"/> from a <see cref="ItemTemplate"/> Template
    /// </summary>
    /// <typeparam name="T">The <see cref="ItemProperties"/> class to return</typeparam>
    /// <param name="template">The <see cref="ItemTemplate"/> Template to get the <see cref="ItemProperties"/> from</param>
    /// <returns>The <see cref="ItemProperties"/> class defined</returns>
    public T GetItemProperties<T>(ItemTemplate template) where T : ItemProperties
    {
        var reader = template.Props.CreateReader();
        var serializer = JsonSerializer.Create(Json.jsonSerializerSettings);

        return serializer.Deserialize<T>(reader);
    }

    public async Task<List<ItemInstance>> CreateItemAsync(ItemTemplate template, int? count = null, MongoId? id = null, string parentId = null,
        string slotId = null)
    {
        var items = new List<ItemInstance>();
        var itemCount = count.GetValueOrDefault(1);

        for (var i = 0; i < itemCount; i++)
        {
            var itemId = i == 0 && id.HasValue ? id.Value : MongoId.Generate();
            var upd = CreateItemUpdatable(template);

            var item = new ItemInstance
            {
                Id = itemId,
                TemplateId = template.Id,
                ParentId = parentId,
                SlotId = slotId,
                Updatable = upd
            };

            items.Add(item);

            var compoundItemProperties = template.Props.ToObject<CompoundItemItemProperties>();

            // Handle child items - these are created once per root item
            if (compoundItemProperties.Slots != null)
            {
                foreach (var slot in compoundItemProperties.Slots.Where(s => s.Required && s.Properties.Filters.Count > 0))
                {
                    if (!slot.Properties.Filters[0].Plate.HasValue)
                    {
                        continue;
                    }

                    var templateId = slot.Properties.Filters[0].Plate.Value;
                    var childTemplate = await _itemTemplates.GetItemTemplateAsync(templateId);
                    var subItems = await CreateItemAsync(childTemplate, null, null, itemId, slot.Name);

                    items.AddRange(subItems);
                }
            }
        }

        return items;
    }

    public ItemUpdatable CreateItemUpdatable(ItemTemplate template, int count = 1)
    {
        ItemUpdatable upd = null;

        if (count > 1)
        {
            upd = new ItemUpdatable { StackObjectsCount = count };
        }

        var updProperties = typeof(ItemUpdatable).GetProperties();

        foreach (var updProperty in updProperties)
        {
            var component = CreateItemComponent(template, updProperty.PropertyType, false);

            if (component != null)
            {
                if (upd == null)
                {
                    upd = new ItemUpdatable();
                }

                updProperty.SetValue(upd, component);
            }
        }

        return upd;
    }

    public object CreateItemComponent(ItemTemplate template, Type componentType, bool createDefault)
    {
        // This means we can't deserialize the type from an ItemTemplate
        if (!typeof(IItemComponent).IsAssignableFrom(componentType))
        {
            return null;
        }

        var createComponentMethod = componentType.GetMethod(nameof(IItemComponent.CreateComponent),
            BindingFlags.Public | BindingFlags.Static);

        if (createComponentMethod == null)
        {
            return null;
        }

        var result = createComponentMethod.Invoke(null, [template.Props]);

        if (result == null && createDefault)
        {
            result = Activator.CreateInstance(componentType);
        }

        return result;
    }

    public async Task<ItemUpdatable> CreateItemUpdatableAsync(MongoId tpl)
    {
        var template = await _itemTemplates.GetItemTemplateAsync(tpl);
        return CreateItemUpdatable(template);
    }

    public async Task<object> CreateItemComponentAsync(MongoId tpl, Type componentType, bool createDefault)
    {
        var template = await _itemTemplates.GetItemTemplateAsync(tpl);
        return CreateItemComponent(template, componentType, createDefault);
    }

    public async Task<List<List<ItemInstance>>> CreateItemsFromTradeRequestAsync(List<ItemInstance> purchasedItem, int count)
    {
        if (purchasedItem == null)
        {
            throw new ArgumentNullException(nameof(purchasedItem));
        }

        if (purchasedItem.Count == 0)
        {
            throw new Exception($"{nameof(purchasedItem)}.Count == 0");
        }

        var stacks = new List<List<ItemInstance>>();
        var rootItemProperties = await GetItemPropertiesAsync<ItemProperties>(purchasedItem[0].TemplateId);
        var maxCount = rootItemProperties.StackMaxSize;
        var fullStacks = count / maxCount;
        var remainingItems = count % maxCount;

        for (var i = 0; i < fullStacks; i++)
        {
            var itemStack = Json.Clone<List<ItemInstance>>(purchasedItem);
            if (itemStack[0].Updatable is null)
            {
                itemStack[0].Updatable = new ItemUpdatable();
            }

            itemStack[0].Updatable.StackObjectsCount = maxCount;
            ItemService.RegenerateItemIds(itemStack);
            stacks.Add(itemStack);
        }

        if (remainingItems > 0)
        {
            var itemStack = Json.Clone<List<ItemInstance>>(purchasedItem);
            if (itemStack[0].Updatable is null)
            {
                itemStack[0].Updatable = new ItemUpdatable();
            }

            itemStack[0].Updatable.StackObjectsCount = remainingItems;
            ItemService.RegenerateItemIds(itemStack);
            stacks.Add(itemStack);
        }

        return stacks;
    }
}