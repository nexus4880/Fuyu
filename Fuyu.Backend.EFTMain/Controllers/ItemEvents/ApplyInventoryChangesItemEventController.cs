using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class ApplyInventoryChangesItemEventController : AbstractItemEventController<ApplyInventoryChangesEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;
    private readonly ItemFactoryService _itemFactoryService;
    private static readonly bool _regenerateMatrix = true;

    public ApplyInventoryChangesItemEventController(
        IProfileRepository profiles,
        ItemService itemService,
        ItemFactoryService itemFactoryService
        ) : base("ApplyInventoryChanges")
    {
        _profiles = profiles;
        _itemService = itemService;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, ApplyInventoryChangesEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        // I would definitely like to not regenerate, but we'll see later
        // -- nexus4880, 2025-1-27
        if (_regenerateMatrix)
        {
            await ReinitializeMatrixAsync(profile, request);
        }
        else
        {
            await UtilizeSameMatrix(profile, request);
        }
    }

    private async Task ReinitializeMatrixAsync(EftProfile profile, ApplyInventoryChangesEvent request)
    {
        var profileItems = new ThreadDictionary<MongoId, ItemInstance>(profile.Pmc.Inventory.ItemsMap);

        foreach (var changedItem in request.ChangedItems)
        {
            // Update items to new positions
            if (profileItems.TryGet(changedItem.Id, out var item))
            {
                item.SlotId = changedItem.SlotId;
                item.Location = changedItem.Location;
                item.ParentId = changedItem.ParentId;
            }
        }

        var stashItem = profile.Pmc.Inventory.StashItem;
        var props = await _itemFactoryService
            .GetItemPropertiesAsync<CompoundItemItemProperties>(stashItem.TemplateId);

        // Reinitialize the matrix based on new item positions
        stashItem.InitializeMatrices(props.Grids, profile.Pmc.Inventory.Items);
    }

    private async Task UtilizeSameMatrix(EftProfile profile, ApplyInventoryChangesEvent request)
    {
        foreach (var changedItem in request.ChangedItems)
        {
            var itemAndChildren = profile.Pmc.Inventory.GetItemAndChildren(_itemService, changedItem);

            // Store sorted Location (MUST BE AN UNION.VALUE1)
            var previousLocation = changedItem.Location.Value1;

            // Free all items previous positions
            await profile.Pmc.Inventory.MoveItemAsync(_itemService, _itemFactoryService, itemAndChildren, changedItem.ParentId, changedItem.SlotId, null);

            // MoveItem will overwrite the ItemInstance.Location to null
            // we want to set it back immediately after
            changedItem.Location = previousLocation;
        }

        foreach (var changedItem in request.ChangedItems)
        {
            var itemAndChildren = profile.Pmc.Inventory.GetItemAndChildren(_itemService, changedItem);

            // Set new positions as taken
            await profile.Pmc.Inventory.MoveItemAsync(_itemService, _itemFactoryService, itemAndChildren, changedItem.ParentId, changedItem.SlotId, changedItem.Location.Value1);
        }
    }
}