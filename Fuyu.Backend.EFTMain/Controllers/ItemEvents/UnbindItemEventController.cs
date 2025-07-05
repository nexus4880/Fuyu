using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class UnbindItemEventController : AbstractItemEventController<UnbindItemEvent>
{
    private readonly IProfileRepository _profiles;

    public UnbindItemEventController(IProfileRepository profiles) : base("Unbind")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, UnbindItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        if (!profile.Pmc.Inventory.FastPanel.TryGetValue(request.Index, out var boundItemId))
        {
            context.AppendInventoryError("Nothing is bound to that slot on the backend");

            return;
        }

        if (boundItemId != request.Item)
        {
            context.AppendInventoryError("Received item is not what is bound on the backend");

            return;
        }

        profile.Pmc.Inventory.FastPanel.Remove(request.Index);

        return;
    }
}