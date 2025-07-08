using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class EatItemEventController : AbstractItemEventController<EatItemEvent>
{
    private readonly ILogger<EatItemEventController> _logger;
    private readonly IProfileRepository _profiles;
    private readonly ItemFactoryService _itemFactoryService;
    private readonly ItemService _itemService;

    public EatItemEventController(
        ILogger<EatItemEventController> logger,
        IProfileRepository profiles,
        ItemFactoryService itemFactoryService,
        ItemService itemService
        ) : base("Eat")
    {
        _logger = logger;
        _profiles = profiles;
        _itemFactoryService = itemFactoryService;
        _itemService = itemService;
    }

    // This method only finds the item, as well as the index. Actually consuming/deleting the item needs to be done.
    public override async Task RunAsync(ItemEventContext context, EatItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var item = profile.Pmc.Inventory.FindItem(request.Item);

        if (item == null)
        {
            _logger.LogError("Failed to find item {Id}", request.Item);
            return;
        }

        var foodDrink = await item.GetOrCreateUpdatableAsync<ItemFoodDrinkComponent>(_itemFactoryService);
        if (foodDrink == null)
        {
            _logger.LogError("Could not find ItemFoodDrinkComponent on item: {Id}", request.Item);
            return;
        }

        foodDrink.HpPercent -= request.Count;
        if (foodDrink.HpPercent <= 0)
        {
            await profile.Pmc.Inventory.RemoveItemAsync(_itemService, item);
        }
    }
}