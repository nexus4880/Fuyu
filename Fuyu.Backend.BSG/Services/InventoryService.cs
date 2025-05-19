using System;
using System.Collections.Generic;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Services;

public class InventoryService
{
    public static InventoryService Instance => instance.Value;
    private static readonly Lazy<InventoryService> instance = new(() => new InventoryService());

    private readonly ItemService _itemService;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private InventoryService()
    {
        _itemService = ItemService.Instance;
    }

    // NOTE:
    // * order is really important here!
    // -- seionmoya, 2024-10-24
    public void RegenerateIds(InventoryInfo inventory)
    {
        var mapping = new Dictionary<string, string>();

        // regenerate inventory equipment
        mapping.Add(inventory.Equipment, MongoId.Generate());
        inventory.Equipment = mapping[inventory.Equipment];

        // regenerate inventory stash
        if (inventory.Stash.HasValue)
        {
            mapping.Add(inventory.Stash.Value, MongoId.Generate());
            inventory.Stash = mapping[inventory.Stash.Value];
        }

        // regenerate inventory quest raid items
        if (inventory.QuestRaidItems.HasValue)
        {
            mapping.Add(inventory.QuestRaidItems.Value, MongoId.Generate());
            inventory.QuestRaidItems = mapping[inventory.QuestRaidItems.Value];
        }

        // regenerate inventory quest stash items
        if (inventory.QuestStashItems.HasValue)
        {
            mapping.Add(inventory.QuestStashItems.Value, MongoId.Generate());
            inventory.QuestStashItems = mapping[inventory.QuestStashItems.Value];
        }

        // regenerate inventory sorting table
        if (inventory.SortingTable.HasValue)
        {
            mapping.Add(inventory.SortingTable.Value, MongoId.Generate());
            inventory.SortingTable = mapping[inventory.SortingTable.Value];
        }

        // regenerate inventory items
        if (inventory.Items != null)
        {
            foreach (var id in inventory.ItemsMap.Keys)
            {
                if (!mapping.ContainsKey(id))
                {
                    mapping.Add(id, MongoId.Generate());
                }
            }

            _itemService.RegenerateItemIds(inventory.Items, mapping);
        }

        // regenerate inventory fastpanel
        if (inventory.FastPanel != null)
        {
            foreach (var kvp in inventory.FastPanel)
            {
                inventory.FastPanel[kvp.Key] = mapping[kvp.Value];
            }
        }

        // regenerate inventory favorite items
        if (inventory.FavoriteItems != null)
        {
            for (var i = 0; i < inventory.FavoriteItems.Count; ++i)
            {
                var itemId = inventory.FavoriteItems[i];
                inventory.FavoriteItems[i] = mapping[itemId];
            }
        }
    }
}