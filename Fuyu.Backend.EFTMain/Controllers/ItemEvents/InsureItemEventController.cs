using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class InsureEventController : AbstractItemEventController<InsureItemEvent>
{
    private readonly EftOrm _eftOrm;

    public InsureEventController() : base("Insure")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, InsureItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var insuredItems = new List<InsuredItem>(request.Items.Length);

        foreach (var itemIdToInsure in request.Items)
        {
            var itemInstance = profile.Pmc.Inventory.FindItem(itemIdToInsure);

            if (itemInstance == null)
            {
                throw new Exception("Failed to find one or more items on backend");
            }

            insuredItems.Add(new InsuredItem { itemId = itemIdToInsure, tid = request.TraderId });
        }

        profile.Pmc.InsuredItems.AddRange(insuredItems);

        return Task.CompletedTask;
    }
}