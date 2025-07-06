using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.BSG.Repositories;

public class JsonItemTemplateRepository : IItemTemplateRepository
{
    private readonly ThreadDictionary<MongoId, ItemTemplate> _itemTemplates;

    public JsonItemTemplateRepository()
    {
        var itemsText = Resx.GetText("eft", "database.client.items.json");
        var itemTemplates = Json.Parse<Dictionary<MongoId, ItemTemplate>>(itemsText);
        _itemTemplates = new ThreadDictionary<MongoId, ItemTemplate>(itemTemplates);
    }

    public Task<Dictionary<MongoId, ItemTemplate>> GetAllAsync()
    {
        return Task.FromResult(_itemTemplates.ToDictionary());
    }

    public Task<ItemTemplate> GetItemTemplateAsync(MongoId id)
    {
        if (_itemTemplates.TryGet(id, out var itemTemplate))
        {
            return Task.FromResult(itemTemplate);
        }

        throw new Exception($"ItemTemplate not found {id}");
    }
}