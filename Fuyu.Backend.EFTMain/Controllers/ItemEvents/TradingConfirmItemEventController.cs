using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class TradingConfirmEventController : AbstractItemEventController<TradingConfirmItemEvent>
{
    private readonly IProfileRepository _profiles;
    private readonly ItemService _itemService;
    private readonly RagfairService _ragfairService;
    private readonly ItemFactoryService _itemFactoryService;

    public TradingConfirmEventController(IProfileRepository profiles, ItemService itemService, RagfairService ragfairService, ItemFactoryService itemFactoryService) : base("TradingConfirm")
    {
        _profiles = profiles;
        _itemService = itemService;
        _ragfairService = ragfairService;
        _itemFactoryService = itemFactoryService;
    }

    public override Task RunAsync(ItemEventContext context, TradingConfirmItemEvent request)
    {
        Terminal.WriteLine(context.Data.ToString());

        switch (request.Type)
        {
            case "sell_to_trader":
                {
                    return SellToTrader(context, context.GetData<TradingConfirmSellItemEvent>());
                }
            case "buy_from_trader":
                {
                    return BuyFromTrader(context, context.GetData<TradingConfirmBuyItemEvent>());
                }
        }

        throw new Exception($"Unhandled TradingConfirm.Type '{request.Type}'");
    }

    public async Task SellToTrader(ItemEventContext context, TradingConfirmSellItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var inventory = profile.Pmc.Inventory;
        var roubles = inventory.GetItemsByTemplate("5449016a4bdc2d6f028b456f");

        if (roubles.Count == 0)
        {
            context.AppendInventoryError("You don't have any roubles and I'm too dumb to add items so I can't pay you");
            return;
        }

        foreach (var tradingItem in request.Items)
        {
            try
            {
                var removedItems = await inventory.RemoveItemAsync(_itemService, tradingItem.Id);
                context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.AddRange(removedItems);
            }
            catch (Exception ex)
            {
                context.AppendInventoryError(ex.Message);
                return;
            }
        }

        var roublesItem = roubles[0];
        roublesItem.Updatable.StackObjectsCount += request.Price;
        context.Response.ProfileChanges[profile.Pmc._id].Items.Change.Add(roublesItem);

        return;
    }

    public async Task BuyFromTrader(ItemEventContext context, TradingConfirmBuyItemEvent request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var offer = _ragfairService.GetOfferByRootItemId(request.ItemId);

        if (offer == null)
        {
            throw new Exception("Failed to find offer");
        }

        var itemsToBuy = offer.Items;

        if (offer.RootItem.Updatable.StackObjectsCount < request.Count)
        {
            throw new Exception("Trying to buy more than offer has");
        }

        if (!profile.Pmc.TradersInfo.HasValue
            || !profile.Pmc.TradersInfo.Value.IsValue1
            || !profile.Pmc.TradersInfo.Value.Value1.TryGetValue(request.TraderId, out var traderInfo))
        {
            throw new Exception("Failed to get trader info");
        }

        // Deduct items from player inventory
        foreach (var tradingItem in request.Items)
        {
            var itemInstance = profile.Pmc.Inventory.FindItem(tradingItem.Id);

            if (itemInstance == null)
            {
                throw new Exception("Failed to find item in player inventory");
            }

            itemInstance.Updatable.StackObjectsCount -= tradingItem.Count;

            if (itemInstance.Updatable.StackObjectsCount <= 0)
            {
                await profile.Pmc.Inventory.RemoveItemAsync(_itemService, itemInstance);
                context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(itemInstance);
            }
            else
            {
                context.Response.ProfileChanges[profile.Pmc._id].Items.Change.Add(itemInstance);
            }
        }

        var stacks = await _itemFactoryService.CreateItemsFromTradeRequestAsync(itemsToBuy, request.Count);

        // Add new items to player inventory
        foreach (var stack in stacks)
        {
            // Assume horizontal rotation when purchasing items. I'm unsure of live behavior.
            // I think it tries horizontal and then vertical if it doesn't fit?
            (int itemWidth, int itemHeight) = await _itemService.CalculateItemSizeAsync(stack, EItemRotation.Horizontal);
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

        offer.RootItem.Updatable.StackObjectsCount -= request.Count;

        traderInfo.salesSum += offer.RequirementsCost * request.Count;

        context.Response.ProfileChanges[profile.Pmc._id].TradersData[request.TraderId] = new TraderData
        {
            _salesSum = traderInfo.salesSum
        };
    }
}