using System;
using System.Collections.Generic;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain;

public class TraderDatabase
{
    public static TraderDatabase Instance => instance.Value;
    private static readonly Lazy<TraderDatabase> instance = new(() => new TraderDatabase());

    private readonly ThreadDictionary<MongoId, TraderTemplate> _traders;
    private readonly ThreadDictionary<MongoId, TraderAssort> _traderAssort;

    private readonly RagfairService _ragfairService;
    private readonly ItemService _itemService;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private TraderDatabase()
    {
        _traders = new ThreadDictionary<MongoId, TraderTemplate>();
        _traderAssort = new ThreadDictionary<MongoId, TraderAssort>();
        _ragfairService = RagfairService.Instance;
        _itemService = ItemService.Instance;
    }

    public Dictionary<MongoId, TraderTemplate> GetTraderTemplates()
    {
        return _traders.ToDictionary();
    }

    public TraderAssort GetTraderAssort(MongoId traderId)
    {
        if (!_traderAssort.TryGet(traderId, out TraderAssort result))
        {
            result = null;
        }

        return result;
    }

    public void SetTraderTemplate(MongoId id, TraderTemplate traderTemplate)
    {
        _traders.Set(id, traderTemplate);
    }

    public void SetTraderAssort(MongoId id, TraderAssort traderAssort)
    {
        _traderAssort.Set(id, traderAssort);

        // NOTE: I don't know where this code should go
        var traderRagfairUser = new RagfairTraderUser(id);

        foreach (var (itemId, scheme) in traderAssort.BarterScheme)
        {
            var items = _itemService.GetItemAndChildren(traderAssort.Items, itemId);
            var handOverRequirements = new List<HandoverRequirement>();
            var loyaltyLevel = traderAssort.LoyaltyLevelItems[itemId];

            foreach (var requirement in scheme)
            {
                foreach (var requirement2 in requirement)
                {
                    handOverRequirements.Add(new HandoverRequirement
                    {
                        Count = (int)requirement2.Count,
                        TemplateId = requirement2.Template
                    });
                }
            }

            // This is called so that the item will be in the 
            // generated category if it doesn't exist
            HandbookService.Instance.GetPrice(items[0].TemplateId, handOverRequirements[0].Count);

            _ragfairService.CreateAndAddOffer(traderRagfairUser, items, false, handOverRequirements,
                TimeSpan.FromHours(30d), loyaltyLevel: loyaltyLevel);
        }
    }
}