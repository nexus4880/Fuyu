using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class MergeItemEventController : AbstractItemEventController<MergeItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;

    public MergeItemEventController(IProfileRepository profiles, ItemService itemService) : base("Merge")
    {
        _profiles = profiles;
        _itemService = itemService;
    }

    public override async Task RunAsync(ItemEventContext context, MergeItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var source = profile.Pmc.Inventory.FindItem(request.Item);

        if (source == null)
        {
            throw new Exception($"Source item {request.Item} not found on backend");
        }

        var target = profile.Pmc.Inventory.FindItem(request.With);

        if (target == null)
        {
            throw new Exception($"Target item {request.With} not found on backend");
        }

        var removedItems = await profile.Pmc.Inventory.RemoveItemAsync(_itemService, source);

        if (removedItems.Count == 0)
        {
            throw new Exception("Failed to remove source item");
        }

        target.Updatable.StackObjectsCount += source.Updatable.StackObjectsCount;
    }
}