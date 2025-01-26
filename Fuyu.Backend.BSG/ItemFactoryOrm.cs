using System;
using System.Collections.Generic;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG;

public class ItemFactoryOrm
{
    public static ItemFactoryOrm Instance => instance.Value;
    private static readonly Lazy<ItemFactoryOrm> instance = new(() => new ItemFactoryOrm());

    private readonly ItemFactoryDatabase _itemFactoryDatabase;

    public ItemFactoryOrm()
    {
        _itemFactoryDatabase = ItemFactoryDatabase.Instance;
    }

    public void SetItemTemplates(Dictionary<MongoId, ItemTemplate> itemTemplates)
    {
        foreach (var (key, value) in itemTemplates)
        {
            _itemFactoryDatabase.itemTemplates.Set(key, value);
        }
    }

    public Dictionary<MongoId, ItemTemplate> GetItemTemplates()
    {
        return _itemFactoryDatabase.itemTemplates.ToDictionary();
    }

    public ItemTemplate GetItemTemplate(MongoId id)
    {
        return _itemFactoryDatabase.itemTemplates.TryGet(id, out var itemTemplate) ? itemTemplate : null;
    }
}
