using System;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG;

public class ItemFactoryDatabase
{
    public static ItemFactoryDatabase Instance => instance.Value;
    private static readonly Lazy<ItemFactoryDatabase> instance = new(() => new ItemFactoryDatabase());

    internal readonly ThreadDictionary<MongoId, ItemTemplate> itemTemplates;

    private ItemFactoryDatabase()
    {
        itemTemplates = new ThreadDictionary<MongoId, ItemTemplate>();
    }
}