using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RepairItemEventController : AbstractItemEventController<RepairItemEvent>
{
    private readonly EftOrm _eftOrm;

    public RepairItemEventController() : base("Repair")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, RepairItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var targetItem = profile.Pmc.Inventory.FindItem(request.TargetItemId);
        var repairable = targetItem.GetOrCreateUpdatable<ItemRepairableComponent>();

        foreach (var repairKitInfo in request.RepairKitsInfo)
        {
            var repairKit = profile.Pmc.Inventory.FindItem(repairKitInfo.Id);

            if (repairKit == null)
            {
                throw new Exception($"Could not find repair kit with id {repairKitInfo.Id}");
            }

            var repairKitComponent = repairKit.GetOrCreateUpdatable<ItemRepairKitComponent>();
            repairKitComponent.Resource -= repairKitInfo.Count;
            repairable.Durability += repairKitInfo.Count;
        }

        return Task.CompletedTask;
    }
}