using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class ToggleItemEventController : AbstractItemEventController<ToggleItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;

    public ToggleItemEventController(IProfileRepository profiles, ItemService itemService) : base("Toggle")
    {
        _profiles = profiles;
        _itemService = itemService;
    }

    public override async Task RunAsync(ItemEventContext context, ToggleItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var item = await profile.Pmc.Inventory.RemoveItemAsync(_itemService, request.Item);

        if (item == null)
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(new ItemInstance { Id = request.Item });
            context.AppendInventoryError($"Failed to find item on backend: {request.Item}, removing it");

            return;
        }

        // TODO: I'm pretty sure we should have an ItemTogglableComponent?
        // -- nexus4880, 2024-10-27
    }
}