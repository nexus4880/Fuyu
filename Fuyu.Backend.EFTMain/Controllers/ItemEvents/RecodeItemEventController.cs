using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RecodeItemEventController : AbstractItemEventController<RecodeItemEvent>
{
    private readonly EftOrm _eftOrm;

    public RecodeItemEventController() : base("Recode")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, RecodeItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var item = profile.Pmc.Inventory.FindItem(request.Item);

        if (item == null)
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(new ItemInstance { Id = request.Item });
            context.AppendInventoryError($"Failed to find item on backend: {request.Item}, removing it");

            return Task.CompletedTask;
        }

        item.GetOrCreateUpdatable<ItemRecodableComponent>().IsEncoded = request.Encoded;

        return Task.CompletedTask;
    }
}