using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RemoveItemEventController : AbstractItemEventController<RemoveItemEvent>
{
    private readonly EftOrm _eftOrm;

    public RemoveItemEventController() : base("Remove")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, RemoveItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var removedItems = profile.Pmc.Inventory.RemoveItem(request.Item);

        if (removedItems.Count == 0)
        {
            throw new Exception($"Failed to find item on backend: {request.Item}");
        }

        return Task.CompletedTask;
    }
}