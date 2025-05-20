using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class PinLockItemEventController : AbstractItemEventController<PinLockItemEvent>
{
    private readonly EftOrm _eftOrm;

    public PinLockItemEventController() : base("PinLock")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, PinLockItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        if (!profile.Pmc.Inventory.ItemsMap.TryGetValue(request.Item, out var item))
        {
            throw new Exception($"Item {request.Item} not found on server");
        }

        var updatable = item.GetOrCreateUpdatable();
        updatable.PinLockState = request.State;

        return Task.CompletedTask;
    }
}
