using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class FoldItemEventController : AbstractItemEventController<FoldItemEvent>
{
    private readonly EftOrm _eftOrm;
    private readonly ItemService _itemService;

    public FoldItemEventController() : base("Fold")
    {
        _eftOrm = EftOrm.Instance;
        _itemService = ItemService.Instance;
    }

    public override Task RunAsync(ItemEventContext context, FoldItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var items = profile.Pmc.Inventory.GetItemAndChildren(_itemService, request.ItemId);

        if (items.Count == 0)
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(new ItemInstance { Id = request.ItemId });
            context.AppendInventoryError($"Failed to find item on backend: {request.ItemId}, removing it");

            return Task.CompletedTask;
        }

        var rootItem = items[0];
        var rootItemLocation = rootItem.Location.Value1;
        var previousSize = _itemService.CalculateItemSize(items, rootItemLocation.r);
        var parent = profile.Pmc.Inventory.FindItem(rootItem.ParentId);
        var parentMatrix = parent.Matrices[rootItem.SlotId];
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
        rootItem.GetOrCreateUpdatable<ItemFoldableComponent>().Folded = request.Value;

        // Setting it to null here so that it gets recalculated
        rootItem.Size = null;

        // Recalculate with new folded state
        var newSize = _itemService.CalculateItemSize(items, rootItemLocation.r);

        // Occupy new slots
        for (var dy = 0; dy < newSize.height; dy++)
        {
            for (var dx = 0; dx < newSize.width; dx++)
            {
                parentMatrix[x + dx, y + dy] = true;
            }
        }

        return Task.CompletedTask;
    }
}