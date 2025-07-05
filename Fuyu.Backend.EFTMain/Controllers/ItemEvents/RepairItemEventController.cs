using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RepairItemEventController : AbstractItemEventController<RepairItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemFactoryService _itemFactoryService;

    public RepairItemEventController(IProfileRepository profiles, ItemFactoryService itemFactoryService) : base("Repair")
    {
        _profiles = profiles;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, RepairItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var targetItem = profile.Pmc.Inventory.FindItem(request.TargetItemId);
        var repairable = await targetItem.GetOrCreateUpdatableAsync<ItemRepairableComponent>(_itemFactoryService);

        foreach (var repairKitInfo in request.RepairKitsInfo)
        {
            var repairKit = profile.Pmc.Inventory.FindItem(repairKitInfo.Id);

            if (repairKit == null)
            {
                throw new Exception($"Could not find repair kit with id {repairKitInfo.Id}");
            }

            var repairKitComponent = await repairKit.GetOrCreateUpdatableAsync<ItemRepairKitComponent>(_itemFactoryService);
            repairKitComponent.Resource -= repairKitInfo.Count;
            repairable.Durability += repairKitInfo.Count;
        }
    }
}