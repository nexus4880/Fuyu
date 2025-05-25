using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class BindItemEventController : AbstractItemEventController<BindItemEvent>
{
    private readonly EftOrm _eftOrm;

    public BindItemEventController() : base("Bind")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, BindItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);

        profile.Pmc.Inventory.FastPanel[request.Index] = request.Item;

        return Task.CompletedTask;
    }
}