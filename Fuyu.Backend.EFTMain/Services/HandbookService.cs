using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Services;

public class HandbookService
{
    private readonly IGameDataRepository _gameData;
    private readonly MongoId _generatedCategoryId;

    public HandbookService(IGameDataRepository gameData)
    {
        _gameData = gameData;
        _generatedCategoryId = MongoId.Generate();
    }

    public HashSet<HandbookCategory> GetHandbookTree(List<HandbookCategory> categories, MongoId rootId)
    {
        var rootEntry = categories.Find(c => c.Id == rootId);

        if (rootEntry == null)
        {
            return [];
        }

        HashSet<HandbookCategory> result = [rootEntry];
        bool added = true;
        while (added)
        {
            added = false;
            foreach (var category in categories)
            {
                if (category.ParentId.HasValue)
                {
                    if (!result.Contains(category) && result.Any(c => c.Id == category.ParentId.Value))
                    {
                        result.Add(category);
                        added = true;
                    }
                }
            }
        }

        return result;
    }

    /// <param name="price">If null will not create a handbook entry in the event no entry was found</param>
    public async Task<int?> GetPriceAsync(MongoId templateId, int? price = null)
    {
        var handbook = await _gameData.GetHandbookAsync();
        var entry = handbook.Items.Find(i => i.Id == templateId);

        if (entry != null)
        {
            return entry.Price;
        }

        var generatedCategory = handbook.Categories.Find(i => i.Id == _generatedCategoryId);

        if (generatedCategory == null)
        {
            generatedCategory = new HandbookCategory
            {
                Id = _generatedCategoryId,
                ParentId = null,
                Icon = "what",
                Color = "#ff0000"
            };

            handbook.Categories.Add(generatedCategory);
        }

        if (price.HasValue)
        {
            handbook.Items.Add(new HandbookItem
            {
                Id = templateId,
                ParentId = _generatedCategoryId,
                Price = price.Value
            });
        }

        return price;
    }

    public async Task<List<HandbookCategory>> GetAllCategoriesOfTypeAsync(HandbookCategory root)
    {
        var handbook = await _gameData.GetHandbookAsync();
        var result = new List<HandbookCategory> { root };
        var added = true;

        while (added)
        {
            added = false;

            for (var i = 0; i < handbook.Categories.Count; i++)
            {
                var category = handbook.Categories[i];

                if (category.ParentId.HasValue &&
                    result.Exists(c => c.Id == category.ParentId.Value) &&
                    !result.Exists(c => c.Id == category.Id))
                {
                    result.Add(category);
                    added = true;
                }
            }
        }

        return result;
    }

    public async Task<List<HandbookItem>> GetAllItemsOfTypeAsync(MongoId id)
    {
        var handbook = await _gameData.GetHandbookAsync();
        var rootCategory = handbook.Categories.Find(c => c.Id == id);
        var categories = (await GetAllCategoriesOfTypeAsync(rootCategory)).Select(c => c.Id).ToList();

        var itemIds = new List<MongoId>();
        var added = true;

        while (added)
        {
            added = false;

            for (var i = 0; i < handbook.Items.Count; i++)
            {
                var item = handbook.Items[i];

                if (!itemIds.Contains(item.Id) && categories.Contains(item.ParentId))
                {
                    itemIds.Add(item.Id);
                    added = true;
                }
            }
        }

        return handbook.Items.Where(i => itemIds.Contains(i.Id)).ToList();
    }

    public async Task<List<HandbookItem>> GetAllItemsOfTypeAndSubcategoriesAsync(MongoId id)
    {
        var handbook = await _gameData.GetHandbookAsync();
        var items = new List<HandbookItem>();

        for (var i = 0; i < handbook.Items.Count; i++)
        {
            var item = handbook.Items[i];

            if (item.ParentId == id)
            {
                items.Add(item);
            }
        }

        return items;
    }
}