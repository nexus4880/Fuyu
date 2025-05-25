using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class MergeItemEventController : AbstractItemEventController<MergeItemEvent>
{
    private readonly EftOrm _eftOrm;

    public MergeItemEventController() : base("Merge")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, MergeItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var source = profile.Pmc.Inventory.FindItem(request.Item);

        if (source == null)
        {
            throw new Exception($"Source item {request.Item} not found on backend");
        }

        var target = profile.Pmc.Inventory.FindItem(request.With);

        if (target == null)
        {
            throw new Exception($"Target item {request.With} not found on backend");
        }

        var removedItems = profile.Pmc.Inventory.RemoveItem(source);

        if (removedItems.Count == 0)
        {
            throw new Exception("Failed to remove source item");
        }

        target.Updatable.StackObjectsCount += source.Updatable.StackObjectsCount;

        return Task.CompletedTask;
    }
}