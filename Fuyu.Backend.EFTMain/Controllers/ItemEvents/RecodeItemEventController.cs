using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RecodeItemEventController : AbstractItemEventController<RecodeItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemFactoryService _itemFactoryService;

    public RecodeItemEventController(IProfileRepository profiles, ItemFactoryService itemFactoryService) : base("Recode")
    {
        _profiles = profiles;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, RecodeItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var item = profile.Pmc.Inventory.FindItem(request.Item);

        if (item == null)
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(new ItemInstance { Id = request.Item });
            context.AppendInventoryError($"Failed to find item on backend: {request.Item}, removing it");

            return;
        }

        var upd = await item.GetOrCreateUpdatableAsync<ItemRecodableComponent>(_itemFactoryService);
        upd.IsEncoded = request.Encoded;
    }
}