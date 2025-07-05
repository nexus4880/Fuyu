using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RagFairBuyOfferItemEventController : AbstractItemEventController<RagFairBuyOfferItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly RagfairService _ragfairService;
    private readonly ItemService _itemService;
    private readonly ItemFactoryService _itemFactoryService;

    public RagFairBuyOfferItemEventController(
        IProfileRepository profiles,
        RagfairService ragfairService,
        ItemService itemService,
        ItemFactoryService itemFactoryService
        ) : base("RagFairBuyOffer")
    {
        _profiles = profiles;
        _ragfairService = ragfairService;
        _itemService = itemService;
        _itemFactoryService = itemFactoryService;
    }

    public override async Task RunAsync(ItemEventContext context, RagFairBuyOfferItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        foreach (var buyOffer in request.BuyOffers)
        {
            var fleaOffer = _ragfairService.GetOffer(buyOffer.Id);

            if (fleaOffer == null)
            {
                throw new Exception("Failed to find offer");
            }

            if (fleaOffer.Quantity < buyOffer.Count)
            {
                throw new Exception("User wants to buy more than available");
            }

            fleaOffer.Quantity -= buyOffer.Count;

            if (fleaOffer.Quantity <= 0)
            {
                await _ragfairService.RemoveOfferAsync(fleaOffer);
            }

            // Remove items from player stash
            foreach (var itemOffer in buyOffer.ItemOffers)
            {
                var handOverItems = profile.Pmc.Inventory.GetItemAndChildren(_itemService, itemOffer.Id);

                if (handOverItems.Count == 0)
                {
                    throw new Exception($"Failed to find item {itemOffer.Id}");
                }

                var handOverItem = handOverItems[0];

                if (!handOverItem.Updatable.StackObjectsCount.HasValue)
                {
                    throw new Exception($"Item {itemOffer.Id} has no stack objects count");
                }

                if (handOverItem.Updatable.StackObjectsCount < itemOffer.Count)
                {
                    throw new Exception("Stack count mismatch");
                }

                handOverItem.Updatable.StackObjectsCount -= itemOffer.Count;

                if (handOverItem.Updatable.StackObjectsCount.Value <= 0)
                {
                    await profile.Pmc.Inventory.RemoveItemAsync(_itemService, handOverItem);
                    context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(handOverItem);
                }
                else
                {
                    context.Response.ProfileChanges[profile.Pmc._id].Items.Change.Add(handOverItem);
                }
            }

            var stacks = await _itemFactoryService.CreateItemsFromTradeRequestAsync(fleaOffer.Items, buyOffer.Count);

            foreach (var stack in stacks)
            {
                (int itemWidth, int itemHeight) = await _itemService.CalculateItemSizeAsync(stack, BSG.Models.Items.EItemRotation.Horizontal);
                var (targetLocation, gridName) = await profile.Pmc.Inventory.GetNextFreeSlotAsync(_itemService, itemWidth, itemHeight);

                if (targetLocation == null)
                {
                    throw new Exception("No room for item");
                }

                var rootItem = stack[0];
                rootItem.Location = targetLocation;
                rootItem.SlotId = gridName;
                rootItem.ParentId = profile.Pmc.Inventory.Stash;

                await profile.Pmc.Inventory.AddItems(_itemService, stack);

                context.Response.ProfileChanges[profile.Pmc._id].Items.New.AddRange(stack);
            }
        }
    }
}