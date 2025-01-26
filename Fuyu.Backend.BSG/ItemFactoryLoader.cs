using System;
using System.Collections.Generic;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.BSG;

public class ItemFactoryLoader
{
    public static ItemFactoryLoader Instance => instance.Value;
    private static readonly Lazy<ItemFactoryLoader> instance = new(() => new ItemFactoryLoader());

    private readonly ItemFactoryOrm _itemFactoryOrm;

    public ItemFactoryLoader()
    {
        _itemFactoryOrm = ItemFactoryOrm.Instance;
    }

    public void Load()
    {
        var itemsText = Resx.GetText("eft", "database.client.items.json");
        var itemTemplates = Json.Parse<ResponseBody<Dictionary<MongoId, ItemTemplate>>>(itemsText).data;

        _itemFactoryOrm.SetItemTemplates(itemTemplates);
    }
}
