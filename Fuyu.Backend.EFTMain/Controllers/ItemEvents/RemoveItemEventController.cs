using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RemoveItemEventController : AbstractItemEventController<RemoveItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;

    public RemoveItemEventController(IProfileRepository profiles, ItemService itemService) : base("Remove")
    {
        _profiles = profiles;
        _itemService = itemService;
    }

    public override async Task RunAsync(ItemEventContext context, RemoveItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var removedItems = await profile.Pmc.Inventory.RemoveItemAsync(_itemService, request.Item);

        if (removedItems.Count == 0)
        {
            throw new Exception($"Failed to find item on backend: {request.Item}");
        }
    }
}