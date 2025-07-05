using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class SplitItemEventController : AbstractItemEventController<SplitItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;
    private readonly ItemFactoryService _itemFactoryService;

    public SplitItemEventController(IProfileRepository profiles, ItemService itemService, ItemFactoryService itemFactoryService) : base("Split")
    {
        _profiles = profiles;
        _itemService = itemService;
        _itemFactoryService = itemFactoryService;
    }

    // TODO: Clean this up later. Probably using methods on grids themselves.
    public override async Task RunAsync(ItemEventContext context, SplitItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var sourceItemStack = profile.Pmc.Inventory.GetItemAndChildren(_itemService, request.SplitItem);

        if (sourceItemStack.Count == 0)
        {
            throw new Exception($"Failed to find source item {request.SplitItem}");
        }

        var sourceItem = sourceItemStack[0];

        if (sourceItem.Updatable.StackObjectsCount < request.Count)
        {
            throw new Exception($"Stack count mismatch");
        }

        var targetLocationItem = profile.Pmc.Inventory.FindItem(request.Container.Id);

        if (targetLocationItem == null)
        {
            throw new Exception($"Failed to find target container {request.Container.Id}");
        }

        if (request.Container.Location != null)
        {
            var matrix = await targetLocationItem.Matrices.GetMatrixAsync(_itemService, request.Container.Container);
            if (matrix is null)
            {
                throw new Exception($"Failed to get matrix for slot {request.Container.Container}");
            }

            (int width, int height) = await _itemService.CalculateItemSizeAsync(sourceItemStack, request.Container.Location.r);
            var x = request.Container.Location.x;
            var y = request.Container.Location.y;

            for (var dy = 0; dy < height; dy++)
            {
                for (var dx = 0; dx < width; dx++)
                {
                    var tempX = x + dx;
                    var tempY = y + dy;

                    if (matrix[tempX, tempY])
                    {
                        throw new Exception("Overlap");
                    }

                    matrix[tempX, tempY] = true;
                }
            }
        }

        sourceItem.Updatable.StackObjectsCount -= request.Count;

        var newItemStacks =
            await _itemFactoryService.CreateItemsFromTradeRequestAsync(sourceItemStack, request.Count);

        var newItemStack = newItemStacks[0];
        var newItem = newItemStack[0];

        var mapping = new Dictionary<string, string>
        {
            { newItem.Id, request.NewItem }
        };

        ItemService.RegenerateItemIds(newItemStack, mapping);

        foreach (var item in newItemStack)
        {
            profile.Pmc.Inventory.ItemsMap[item.Id] = item;
        }

        newItem.Location = request.Container.Location;
        newItem.ParentId = request.Container.Id;
        newItem.SlotId = request.Container.Container;
    }
}