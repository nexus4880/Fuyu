using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Services;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RagFairAddOfferItemEventController : AbstractItemEventController<RagFairAddOfferItemEvent>
{
    private readonly RagfairService _ragfairService;
    private readonly EftOrm _eftOrm;

    public RagFairAddOfferItemEventController() : base("RagFairAddOffer")
    {
        _ragfairService = RagfairService.Instance;
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, RagFairAddOfferItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var ragfairUser = new RagfairPlayerUser(profile.Pmc);
        var items = new List<ItemInstance>();

        foreach (var itemToSellId in request.Items)
        {
            var itemsRemoved = profile.Pmc.Inventory.RemoveItem(itemToSellId);

            if (itemsRemoved.Count == 0)
            {
                throw new Exception($"Item {itemToSellId} not found on backend");
            }

            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(itemsRemoved[0]);

            items.AddRange(itemsRemoved);
        }

        var offer = _ragfairService.CreateAndAddOffer(ragfairUser, items, request.SellAsPack, request.Requirements,
            TimeSpan.FromMinutes(30d));

        if (offer == null)
        {
            throw new Exception("Failed to create offer");
        }

        return Task.CompletedTask;
    }
}