using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class FoldItemEventController : AbstractItemEventController<FoldItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;
    private readonly ItemFactoryService _itemFactoryService;

    public FoldItemEventController(IProfileRepository profiles, ItemService itemService, ItemFactoryService itemFactoryService) : base("Fold")
    {
        _profiles = profiles;
        _itemService = itemService;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, FoldItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var items = profile.Pmc.Inventory.GetItemAndChildren(_itemService, request.ItemId);

        if (items.Count == 0)
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(new ItemInstance { Id = request.ItemId });
            context.AppendInventoryError($"Failed to find item on backend: {request.ItemId}, removing it");

            return;
        }

        var rootItem = items[0];
        var rootItemLocation = rootItem.Location.Value1;
        var previousSize = await _itemService.CalculateItemSizeAsync(items, rootItemLocation.r);
        var parent = profile.Pmc.Inventory.FindItem(rootItem.ParentId);
        var parentMatrix = await parent.Matrices.GetMatrixAsync(_itemService, rootItem.SlotId);
        var x = rootItemLocation.x;
        var y = rootItemLocation.y;

        // Free old slots
        for (var dy = 0; dy < previousSize.height; dy++)
        {
            for (var dx = 0; dx < previousSize.width; dx++)
            {
                parentMatrix[x + dx, y + dy] = false;
            }
        }

        // Assign folded state
        var upd = await rootItem.GetOrCreateUpdatableAsync<ItemFoldableComponent>(_itemFactoryService);
        upd.Folded = request.Value;

        // Setting it to null here so that it gets recalculated
        rootItem.Size = null;

        // Recalculate with new folded state
        var newSize = await _itemService.CalculateItemSizeAsync(items, rootItemLocation.r);

        // Occupy new slots
        for (var dy = 0; dy < newSize.height; dy++)
        {
            for (var dx = 0; dx < newSize.width; dx++)
            {
                parentMatrix[x + dx, y + dy] = true;
            }
        }
    }
}