using System;
using System.Collections.Generic;
using System.Linq;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Services;

public class HandbookService
{
    public static HandbookService Instance => _instance.Value;

    private static readonly Lazy<HandbookService> _instance = new(() => new HandbookService());

    private readonly EftOrm _eftOrm;
    private readonly MongoId _generatedCategoryId;

    public HandbookService()
    {
        _eftOrm = EftOrm.Instance;
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
    public int? GetPrice(MongoId templateId, int? price = null)
    {
        var handbook = _eftOrm.GetHandbook();
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

    public List<HandbookCategory> GetAllCategoriesOfType(HandbookCategory root)
    {
        var handbook = _eftOrm.GetHandbook();
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

    public List<HandbookItem> GetAllItemsOfType(MongoId id)
    {
        var handbook = _eftOrm.GetHandbook();
        var rootCategory = handbook.Categories.Find(c => c.Id == id);
        var categories = GetAllCategoriesOfType(rootCategory).Select(c => c.Id).ToList();

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

    public List<HandbookItem> GetAllItemsOfTypeAndSubcategories(MongoId id)
    {
        var handbook = _eftOrm.GetHandbook();
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