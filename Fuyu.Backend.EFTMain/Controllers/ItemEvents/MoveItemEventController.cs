using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class MoveItemEventController : AbstractItemEventController<MoveItemEvent>
{
    private readonly EftOrm _eftOrm;

    public MoveItemEventController() : base("Move")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, MoveItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var items = profile.Pmc.Inventory.GetItemAndChildren(ItemService.Instance, request.Item);

        if (items.Count == 0)
        {
            throw new Exception($"Failed to find {request.Item} in inventory");
        }

        profile.Pmc.Inventory.MoveItem(
            items,
            request.To.Id,
            request.To.Container,
            request.To.Location
        );

        return Task.CompletedTask;
    }
}