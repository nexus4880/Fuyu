using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class TransferItemEventController : AbstractItemEventController<TransferItemEvent>
{
    private readonly EftOrm _eftOrm;

    public TransferItemEventController() : base("Transfer")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, TransferItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var item = profile.Pmc.Inventory.FindItem(request.Item);

        if (item == null)
        {
            throw new Exception($"Source item {request.Item} not found on backend");
        }

        if (item.Updatable.StackObjectsCount < request.Count)
        {
            throw new Exception(
                $"Client-backend count mismatch. Got {request.Count}, have {item.Updatable.StackObjectsCount.Value}"
            );
        }

        item.Updatable.StackObjectsCount -= request.Count;

        var with = profile.Pmc.Inventory.FindItem(request.With);

        if (with == null)
        {
            throw new Exception($"Target item {request.With} not found on backend");
        }

        with.Updatable.StackObjectsCount += request.Count;

        return Task.CompletedTask;
    }
}