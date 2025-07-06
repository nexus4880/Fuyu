using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class BindItemEventController : AbstractItemEventController<BindItemEvent>
{
    private readonly IProfileRepository _profiles;

    public BindItemEventController(IProfileRepository profiles) : base("Bind")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(ItemEventContext context, BindItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        profile.Pmc.Inventory.FastPanel[request.Index] = request.Item;
    }
}