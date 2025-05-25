using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class ToggleItemEventController : AbstractItemEventController<ToggleItemEvent>
{
    private readonly EftOrm _eftOrm;

    public ToggleItemEventController() : base("Toggle")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, ToggleItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var item = profile.Pmc.Inventory.RemoveItem(request.Item);

        if (item == null)
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(new ItemInstance { Id = request.Item });
            context.AppendInventoryError($"Failed to find item on backend: {request.Item}, removing it");

            return Task.CompletedTask;
        }

        // TODO: I'm pretty sure we should have an ItemTogglableComponent?
        // -- nexus4880, 2024-10-27

        return Task.CompletedTask;
    }
}