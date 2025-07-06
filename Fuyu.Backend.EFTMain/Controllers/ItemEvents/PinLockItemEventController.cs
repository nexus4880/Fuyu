using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class PinLockItemEventController : AbstractItemEventController<PinLockItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemFactoryService _itemFactoryService;

    public PinLockItemEventController(IProfileRepository profiles, ItemFactoryService itemFactoryService) : base("PinLock")
    {
        _profiles = profiles;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, PinLockItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        if (!profile.Pmc.Inventory.ItemsMap.TryGetValue(request.Item, out var item))
        {
            throw new Exception($"Item {request.Item} not found on server");
        }

        var updatable = await item.GetOrCreateUpdatableAsync(_itemFactoryService);
        updatable.PinLockState = request.State;
    }
}