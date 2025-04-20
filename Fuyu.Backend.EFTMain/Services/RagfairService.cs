using System;
using System.Collections.Generic;
using System.Linq;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Services;

public class RagfairService
{
    private static readonly Lazy<RagfairService> instance = new(() => new RagfairService());

    private readonly EftOrm _eftOrm;
    private readonly HandbookService _handbookService;

    public Dictionary<MongoId, int> CategoricalOffers { get; } = [];

    private RagfairService()
    {
        _eftOrm = EftOrm.Instance;
        _handbookService = HandbookService.Instance;
    }

    public static RagfairService Instance => instance.Value;

    public List<Offer> Offers { get; } = [];

    public Offer CreateAndAddOffer(IRagfairUser user, List<ItemInstance> items, bool isBatch,
        List<HandoverRequirement> requirements, TimeSpan lifetime, int quantity = 1, bool unlimitedCount = false, int loyaltyLevel = 1)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (requirements == null)
        {
            throw new ArgumentNullException(nameof(requirements));
        }

        if (items.Count == 0)
        {
            throw new Exception($"{nameof(items)} is empty");
        }

        if (requirements.Count == 0)
        {
            throw new Exception($"{nameof(requirements)} is empty");
        }

        var cost = items.Sum(i => _handbookService.GetPrice(i.TemplateId, 100).Value);

        if (isBatch)
        {
            cost *= quantity;
        }

        var offer = new Offer()
        {
            Id = MongoId.Generate(),
            IntId = Offers.Count,
            User = user,
            RootItemId = items[0].Id,
            Items = items,
            ItemsCost = cost,
            Requirements = requirements,
            RequirementsCost =
                requirements.Sum(i => _handbookService.GetPrice(i.TemplateId).GetValueOrDefault(1) * i.Count),
            SummaryCost = 0,
            SellInOnePiece = isBatch,
            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d,
            EndTime = (DateTimeOffset.UtcNow + lifetime).ToUnixTimeMilliseconds() / 1000d,
            UnlimitedCount = unlimitedCount,
            LoyaltyLevel = loyaltyLevel,
            Quantity = quantity
        };

        return AddOffer(offer);
    }

    public Offer GetOffer(MongoId offerId)
    {
        return Offers.Find(o => o.Id == offerId);
    }

    public Offer AddOffer(Offer offer)
    {
        var items = offer.Items;
        var handbook = _eftOrm.GetHandbook();
        var handbookItem = handbook.Items.Find(i => i.Id == items[0].TemplateId);

        if (!CategoricalOffers.TryAdd(handbookItem.ParentId, 1))
        {
            CategoricalOffers[handbookItem.ParentId]++;
        }

        if (!CategoricalOffers.TryAdd(items[0].TemplateId, 1))
        {
            CategoricalOffers[items[0].TemplateId]++;
        }

        Offers.Add(offer);

        return offer;
    }

    public void RemoveOffer(Offer offer)
    {
        if (!Offers.Remove(offer))
        {
            throw new Exception($"Failed to remove offer {offer.Id}");
        }

        var handbook = _eftOrm.GetHandbook();
        var handbookItem = handbook.Items.Find(i => i.Id == offer.RootItem.TemplateId);

        if (handbookItem != null)
        {
            if (!CategoricalOffers.ContainsKey(handbookItem.ParentId))
            {
                CategoricalOffers[handbookItem.ParentId]--;
            }
        }

        if (!CategoricalOffers.ContainsKey(offer.RootItem.TemplateId))
        {
            CategoricalOffers[offer.RootItem.TemplateId]--;
        }
    }

    public Offer GetOfferByRootItemId(MongoId id)
    {
        return Offers.Find(o => o.RootItemId == id);
    }
}