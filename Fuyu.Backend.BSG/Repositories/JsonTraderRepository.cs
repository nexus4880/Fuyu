using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.BSG.Services;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.BSG.Repositories;

public class JsonTraderRepository : ITraderRepository
{
    private readonly ILogger<JsonTraderRepository> _logger;
    private readonly ThreadDictionary<MongoId, TraderTemplate> _traderTemplates;
    private readonly ThreadDictionary<MongoId, TraderAssort> _traderAssort;

    public JsonTraderRepository(ILogger<JsonTraderRepository> logger, RagfairService ragfairService, ItemService itemService)
    {
        _logger = logger;
        _traderTemplates = new ThreadDictionary<MongoId, TraderTemplate>();
        _traderAssort = new ThreadDictionary<MongoId, TraderAssort>();

        var tradersJson = Resx.GetText("eft", "database.client.trading.api.traderSettings.json");
        var body = Json.Parse<TraderTemplate[]>(tradersJson);

        foreach (var traderTemplate in body)
        {
            _traderTemplates.Set(traderTemplate.Id, traderTemplate);

            string assortJson;

            try
            {
                assortJson = Resx.GetText("eft",
                    $"database.client.trading.api.getTraderAssort.{traderTemplate.Id}.json");
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("Failed to get assort for {TraderId}", traderTemplate.Id);
                continue;
            }

            var traderAssort = Json.Parse<TraderAssort>(assortJson);
            _traderAssort.Set(traderTemplate.Id, traderAssort);
            _logger.LogInformation("Got assort for {TraderId}", traderTemplate.Id);
            var traderUser = new RagfairTraderUser(traderTemplate.Id);
            foreach (var (id, scheme) in traderAssort.BarterScheme)
            {
                var items = itemService.GetItemAndChildren(traderAssort.Items, id);
                ragfairService.CreateAndAddOffer(traderUser, items, false, scheme.SelectMany(s =>
                {
                    return s.Select(h => new HandoverRequirement
                    {
                        TemplateId = h.Template,
                        Count = (int)h.Count
                    });
                }).ToList(), TimeSpan.FromDays(1d), items[0].Updatable?.StackObjectsCount ?? 1, false, 1)
                    .GetAwaiter().GetResult();
            }
        }
    }

    public Task<Dictionary<MongoId, TraderAssort>> GetTraderAssortsAsync()
    {
        return Task.FromResult(_traderAssort.ToDictionary());
    }

    public Task<Dictionary<MongoId, TraderTemplate>> GetTraderTemplatesAsync()
    {
        return Task.FromResult(_traderTemplates.ToDictionary());
    }

    public Task<TraderTemplate> GetTraderTemplateAsync(MongoId id)
    {
        if (_traderTemplates.TryGet(id, out var traderTemplate))
        {
            return Task.FromResult(traderTemplate);
        }

        throw new Exception($"Failed to get trader template from id: {id}");
    }

    public Task<TraderAssort> GetTraderAssortAsync(MongoId id)
    {
        if (_traderAssort.TryGet(id, out var traderAssort))
        {
            return Task.FromResult(traderAssort);
        }

        throw new Exception($"Failed to get trader assort from id: {id}");
    }
}