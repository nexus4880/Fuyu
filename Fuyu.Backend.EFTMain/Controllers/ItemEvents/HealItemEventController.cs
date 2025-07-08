using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class HealItemEventController : AbstractItemEventController<HealItemEvent>
{
    private readonly ILogger<HealItemEventController> _logger;
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;
    private readonly ItemFactoryService _itemFactoryService;

    public HealItemEventController(
        ILogger<HealItemEventController> logger,
        IProfileRepository profiles,
        ItemService itemService,
        ItemFactoryService itemFactoryService
        ) : base("Heal")
    {
        _logger = logger;
        _profiles = profiles;
        _itemService = itemService;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, HealItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var item = profile.Pmc.Inventory.FindItem(request.Item);

        if (item == null)
        {
            _logger.LogError("Failed to find item {ItemId}", request.Item);
            return;
        }

        var medKit = await item.GetOrCreateUpdatableAsync<ItemMedKitComponent>(_itemFactoryService);
        var bodyPart = profile.Pmc.Health.GetBodyPart(request.BodyPart);
        float toHeal = request.Count;

        if (profile.Pmc.Health.HasEffects)
        {
            var itemProperties = await _itemFactoryService.GetItemPropertiesAsync<MedsItemProperties>(item.TemplateId);

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
            await profile.Pmc.Inventory.RemoveItemAsync(_itemService, item);
        }

        // TODO:
        // Check BackendConfig for 'HealExperience' and add to PMC profile

    }
}