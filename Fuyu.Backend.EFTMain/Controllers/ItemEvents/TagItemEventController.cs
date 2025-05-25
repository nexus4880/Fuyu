using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class TagItemEventController : AbstractItemEventController<TagItemEvent>
{
    private readonly EftOrm _eftOrm;

    public TagItemEventController() : base("Tag")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, TagItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var item = profile.Pmc.Inventory.FindItem(request.Item);

        if (item != null)
        {
            var tag = item.GetOrCreateUpdatable<ItemTagComponent>();
            tag.Name = request.Name;
            tag.Color = request.Color;
        }
        else
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(new ItemInstance { Id = request.Item });
            context.AppendInventoryError($"Failed to find item on backend: {request.Item}, removing it");
        }

        return Task.CompletedTask;
    }
}