using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class HealItemEventController : AbstractItemEventController<HealItemEvent>
{
    private readonly EftOrm _eftOrm;
    private readonly ItemFactoryService _itemFactoryService;

    public HealItemEventController() : base("Heal")
    {
        _eftOrm = EftOrm.Instance;
        _itemFactoryService = ItemFactoryService.Instance;
    }

    public override Task RunAsync(ItemEventContext context, HealItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var item = profile.Pmc.Inventory.FindItem(request.Item);

        if (item == null)
        {
            Terminal.WriteLine($"Failed to find item {request.Item}");
            return Task.CompletedTask;
        }

        var medKit = item.GetOrCreateUpdatable<ItemMedKitComponent>();

        var bodyPart = profile.Pmc.Health.GetBodyPart(request.BodyPart);
        float toHeal = request.Count;

        if (profile.Pmc.Health.HasEffects)
        {
            var itemProperties = _itemFactoryService.GetItemProperties<MedsItemProperties>(item.TemplateId);

            if (itemProperties.DamageEffects.IsValue1)
            {
                foreach (var (effectName, effect) in itemProperties.DamageEffects.Value1)
                {
                    if (bodyPart.Effects.ContainsKey(effectName))
                    {
                        toHeal -= effect.Cost;
                        bodyPart.Effects.Remove(effectName);
                    }
                }
            }
        }

        bodyPart.Health.Current += toHeal;
        medKit.HpResource -= request.Count;

        if (medKit.HpResource <= 0)
        {
            profile.Pmc.Inventory.RemoveItem(item);
        }

        // TODO:
        // Check BackendConfig for 'HealExperience' and add to PMC profile

        return Task.CompletedTask;
    }
}