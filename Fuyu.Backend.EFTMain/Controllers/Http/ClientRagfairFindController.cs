using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientRagfairFindController : AbstractEftHttpController<RagfairFindRequest>
{
    private readonly EftOrm _eftOrm;
    private readonly RagfairService _ragfairService;
    private readonly HandbookService _handbookService;
    private readonly ItemFactoryService _itemFactoryService;
    private readonly ItemService _itemService;

    private readonly List<MongoId> _money = new List<MongoId>
    {
        // Roubles
        "5449016a4bdc2d6f028b456f",
        // Dollars
        "5696686a4bdc2da3298b456a",
        // Euros
        "569668774bdc2da2298b4568",
        // GP Coin
        "5d235b4d86f7742e017bc88a"
    };

    public ClientRagfairFindController() : base("/client/ragfair/find")
    {
        _eftOrm = EftOrm.Instance;
        _ragfairService = RagfairService.Instance;
        _handbookService = HandbookService.Instance;
        _itemService = ItemService.Instance;
        _itemFactoryService = ItemFactoryService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, RagfairFindRequest body)
    {
        Terminal.WriteLine(Json.Stringify(body));
        var sw = Stopwatch.StartNew();
        var handbook = _eftOrm.GetHandbook();

        ResponseBody<OffersListResponse> responseBody;
        List<Offer> selectedOffers;
        string selectedCategory;
        Dictionary<MongoId, int> categories = _ragfairService.CategoricalOffers;
        var isLinkedSearch = body.LinkedSearchId.HasValue;

        if (body.HandbookId.HasValue)
        {
            selectedOffers = SearchByItem(handbook, body.HandbookId.Value);
            selectedCategory = body.HandbookId;
        }
        else if (body.LinkedSearchId.HasValue)
        {
            // TODO: when linked searching selectedCategory should be one of the categories
            // of one of the available template ids (linked search mag should return bullets
            // and selectedCategory should be ammo)
            // -- nexus4880, 2025-1-26
            selectedOffers = LinkedSearch(handbook, body.LinkedSearchId.Value, out selectedCategory);
            selectedCategory = body.LinkedSearchId;
        }
        else if (body.NeededSearchId.HasValue)
        {
            selectedOffers = RequiredSearch(handbook, body.NeededSearchId.Value, out selectedCategory);
            selectedCategory = body.NeededSearchId;
        }
        else
        {
            responseBody = new ResponseBody<OffersListResponse>() { errmsg = "Improper request" };
            goto sendResponse;
        }

        switch (body.OfferOwnerType)
        {
            // All, just leaving to show that
            case 0:
                {
                    break;
                }
            // Traders only
            case 1:
                {
                    selectedOffers.RemoveAll(o => !o.User.MemberCategory.HasFlag(EMemberCategory.Trader));
                    break;
                }
            // Players only
            case 2:
                {
                    selectedOffers.RemoveAll(o => o.User.MemberCategory.HasFlag(EMemberCategory.Trader));
                    break;
                }
        }

        if (body.RemoveBartering)
        {
            selectedOffers.RemoveAll(o => o.Requirements.Any(i => !_money.Contains(i.TemplateId)));
        }

        if (body.Currency > 0)
        {
            var targetCurrency = _money[body.Currency - 1];

            selectedOffers.RemoveAll(o => o.Requirements.TrueForAll(i => i.TemplateId != targetCurrency));
        }

        if (body.QuantityFrom > 0)
        {
            selectedOffers.RemoveAll(o => o.RootItem.Updatable.StackObjectsCount < body.QuantityFrom);
        }

        if (body.QuantityTo > 0)
        {
            selectedOffers.RemoveAll(o => o.RootItem.Updatable.StackObjectsCount > body.QuantityTo);
        }

        if (body.ConditionFrom > 0 || body.ConditionTo < 100)
        {
            selectedOffers.RemoveAll(o => !MeetsConditions(o, body.ConditionTo, body.ConditionFrom));
        }

        if (body.PriceFrom > 0)
        {
            selectedOffers.RemoveAll(o => o.RequirementsCost < body.PriceFrom);
        }

        if (body.PriceTo > 0)
        {
            selectedOffers.RemoveAll(o => o.RequirementsCost > body.PriceTo);
        }

        if (body.OnlyFunctional)
        {
            // Maybe only needs to run on RootItem?
            selectedOffers.RemoveAll(o => !o.Items.TrueForAll(i => _itemService.IsFunctional(o.Items, i)));
        }

        // Ascending
        if (body.SortDirection == 0)
        {
            selectedOffers.Sort((a, b) => a.RequirementsCost - b.RequirementsCost);
        }
        // Descending
        else
        {
            selectedOffers.Sort((a, b) => b.RequirementsCost - a.RequirementsCost);
        }

        // Moves the enumerator N times (essentially removing them from the list)
        var offers = selectedOffers.Skip(body.Page * body.Limit);

        // Creates a slice of the next N elements
        var finalOffers = offers.Take(body.Limit).ToList();

        if (isLinkedSearch)
        {
            categories = new Dictionary<MongoId, int>();

            foreach (var (id, count) in _ragfairService.CategoricalOffers)
            {
                if (finalOffers.Exists(offer => offer.Items.Exists(j => id == j.TemplateId)))
                {
                    categories[id] = count;
                }
            }
        }

        responseBody = new ResponseBody<OffersListResponse>()
        {
            data = new OffersListResponse
            {
                Categories = categories,
                Offers = finalOffers,
                OffersCount = selectedOffers.Count,
                SelectedCategory = selectedCategory
            }
        };

        Terminal.WriteLine(sw.ElapsedMilliseconds);

    sendResponse:
        return context.SendResponseAsync(responseBody, true, true);
    }

    private List<Offer> SearchByItem(HandbookTemplates handbook, MongoId handbookId)
    {
        HashSet<MongoId> templateIds = [];
        List<Offer> selectedOffers = [];

        var rootItemEntry = handbook.Items.Find(i => i.Id == handbookId);

        if (rootItemEntry != null)
        {
            templateIds.Add(handbookId);
        }
        else
        {
            HashSet<HandbookCategory> categories = _handbookService.GetHandbookTree(handbook.Categories, handbookId);

            foreach (var handbookItem in handbook.Items)
            {
                if (categories.FirstOrDefault(i => i.Id == handbookItem.ParentId) != null)
                {
                    templateIds.Add(handbookItem.Id);
                }
            }
        }

        foreach (var offer in _ragfairService.Offers)
        {
            if (templateIds.Contains(offer.RootItem.TemplateId))
            {
                selectedOffers.Add(offer);
            }
        }

        return selectedOffers;
    }

    private List<Offer> LinkedSearch(HandbookTemplates handbook, MongoId linkedSearchId, out string selectedCategory)
    {
        var handbookItem = handbook.Items.Find(i => i.Id == linkedSearchId);

        if (handbookItem == null)
        {
            throw new Exception($"Failed to find handbook entry for {linkedSearchId}");
        }

        var handbookCategory = handbook.Categories.Find(i => i.Id == handbookItem.ParentId);

        if (handbookCategory == null)
        {
            throw new Exception($"Failed to find category of {handbookItem.ParentId} for {handbookItem.Id}");
        }

        selectedCategory = handbookCategory.Id;

        var rootItemTemplate = ItemFactoryOrm.Instance.GetItemTemplate(linkedSearchId);
        var baseItemProperties = rootItemTemplate.Props;
        var itemProperties = baseItemProperties.ToObject<CompoundItemItemProperties>();
        var magazineItemProperties = baseItemProperties.ToObject<MagazineItemProperties>();
        var weaponItemProperties = baseItemProperties.ToObject<WeaponItemProperties>();
        var result = new List<Offer>();

        if (itemProperties.Slots != null)
        {
            foreach (var slot in itemProperties.Slots)
            {
                foreach (var filter in slot.Properties.Filters.SelectMany(f => f.Filter))
                {
                    result.AddRange(SearchByItem(handbook, filter));
                }
            }
        }

        if (magazineItemProperties.Cartridges != null)
        {
            foreach (var cartridge in magazineItemProperties.Cartridges)
            {
                foreach (var filter in cartridge.Properties.Filters.SelectMany(f => f.Filter))
                {
                    result.AddRange(SearchByItem(handbook, filter));
                }
            }
        }

        if (weaponItemProperties.Chambers != null)
        {
            foreach (var chamber in weaponItemProperties.Chambers)
            {
                foreach (var filter in chamber.Properties.Filters.SelectMany(f => f.Filter))
                {
                    result.AddRange(SearchByItem(handbook, filter));
                }
            }
        }

        return result;
    }

    private List<Offer> RequiredSearch(HandbookTemplates handbook, MongoId neededSearchId, out string selectedCategory)
    {
        selectedCategory = null;

        var result = new List<Offer>();

        foreach (var offer in _ragfairService.Offers)
        {
            if (offer.Requirements.Exists(requirement => requirement.TemplateId == neededSearchId))
            {
                result.Add(offer);
            }
        }

        return result;
    }

    private bool MeetsConditions(Offer offer, int conditionFrom, int conditionTo)
    {
        var repairable = offer.RootItem.Updatable.Repairable;

        if (repairable != null)
        {
            var percentage = repairable.Durability / repairable.MaxDurability * 100f;

            if (percentage > conditionTo || percentage < conditionFrom)
            {
                return false;
            }
        }

        var repairKit = offer.RootItem.Updatable.RepairKit;

        if (repairKit != null)
        {
            var properties = _itemFactoryService.GetItemProperties<RepairKitsItemProperties>(offer.RootItem.TemplateId);

            if (properties == null)
            {
                return false;
            }

            var percentage = repairKit.Resource / properties.MaxRepairResource * 100f;

            if (percentage > conditionTo || percentage < conditionFrom)
            {
                return false;
            }
        }

        return true;
    }
}