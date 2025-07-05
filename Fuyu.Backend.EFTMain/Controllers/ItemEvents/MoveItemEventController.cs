using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class MoveItemEventController : AbstractItemEventController<MoveItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;
    private readonly ItemFactoryService _itemFactoryService;

    public MoveItemEventController(IProfileRepository profiles, ItemService itemService, ItemFactoryService itemFactoryService) : base("Move")
    {
        _profiles = profiles;
        _itemService = itemService;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, MoveItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var items = profile.Pmc.Inventory.GetItemAndChildren(_itemService, request.Item);

        if (items.Count == 0)
        {
            throw new Exception($"Failed to find {request.Item} in inventory");
        }


        await profile.Pmc.Inventory.MoveItemAsync(
            _itemService,
            _itemFactoryService,
            items,
            request.To.Id,
            request.To.Container,
            request.To.Location
        );
    }
}