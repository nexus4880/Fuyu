using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class InsureEventController : AbstractItemEventController<InsureItemEvent>
{
    private readonly IProfileRepository _profiles;

    public InsureEventController(IProfileRepository profiles) : base("Insure")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, InsureItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
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
    }
}