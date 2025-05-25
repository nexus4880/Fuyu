using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class UnbindItemEventController : AbstractItemEventController<UnbindItemEvent>
{
    private readonly EftOrm _eftOrm;

    public UnbindItemEventController() : base("Unbind")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, UnbindItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);

        if (!profile.Pmc.Inventory.FastPanel.TryGetValue(request.Index, out var boundItemId))
        {
            context.AppendInventoryError("Nothing is bound to that slot on the backend");

            return Task.CompletedTask;
        }

        if (boundItemId != request.Item)
        {
            context.AppendInventoryError("Received item is not what is bound on the backend");

            return Task.CompletedTask;
        }

        profile.Pmc.Inventory.FastPanel.Remove(request.Index);

        return Task.CompletedTask;
    }
}