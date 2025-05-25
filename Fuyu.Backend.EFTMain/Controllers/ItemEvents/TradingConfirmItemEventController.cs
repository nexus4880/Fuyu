using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class TradingConfirmEventController : AbstractItemEventController<TradingConfirmItemEvent>
{
    private readonly EftOrm _eftOrm;
    private readonly ItemService _itemService;
    private readonly RagfairService _ragfairService;

    public TradingConfirmEventController() : base("TradingConfirm")
    {
        _eftOrm = EftOrm.Instance;
        _itemService = ItemService.Instance;
        _ragfairService = RagfairService.Instance;
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

    public Task SellToTrader(ItemEventContext context, TradingConfirmSellItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var inventory = profile.Pmc.Inventory;
        var roubles = inventory.GetItemsByTemplate("5449016a4bdc2d6f028b456f");

        if (roubles.Count == 0)
        {
            context.AppendInventoryError("You don't have any roubles and I'm too dumb to add items so I can't pay you");

            return Task.CompletedTask;
        }

        foreach (var tradingItem in request.Items)
        {
            try
            {
                var removedItems = inventory.RemoveItem(tradingItem.Id);
                context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.AddRange(removedItems);
            }
            catch (Exception ex)
            {
                context.AppendInventoryError(ex.Message);

                return Task.CompletedTask;
            }
        }

        var roublesItem = roubles[0];
        roublesItem.Updatable.StackObjectsCount += request.Price;
        context.Response.ProfileChanges[profile.Pmc._id].Items.Change.Add(roublesItem);

        return Task.CompletedTask;
    }

    public Task BuyFromTrader(ItemEventContext context, TradingConfirmBuyItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
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
                profile.Pmc.Inventory.RemoveItem(itemInstance);
                context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(itemInstance);
            }
            else
            {
                context.Response.ProfileChanges[profile.Pmc._id].Items.Change.Add(itemInstance);
            }
        }

        var stacks = ItemFactoryService.Instance.CreateItemsFromTradeRequest(itemsToBuy, request.Count);

        // Add new items to player inventory
        foreach (var stack in stacks)
        {
            // Assume horizontal rotation when purchasing items. I'm unsure of live behavior.
            // I think it tries horizontal and then vertical if it doesn't fit?
            (int itemWidth, int itemHeight) = _itemService.CalculateItemSize(stack, EItemRotation.Horizontal);
            var targetLocation = profile.Pmc.Inventory.GetNextFreeSlot(_itemService, itemWidth, itemHeight, out string gridName);

            if (targetLocation == null)
            {
                throw new Exception("No room for item");
            }

            var rootItem = stack[0];

            rootItem.Location = targetLocation;
            rootItem.SlotId = gridName;
            rootItem.ParentId = profile.Pmc.Inventory.Stash;

            profile.Pmc.Inventory.AddItems(_itemService, stack);

            context.Response.ProfileChanges[profile.Pmc._id].Items.New.AddRange(stack);
        }

        offer.RootItem.Updatable.StackObjectsCount -= request.Count;

        traderInfo.salesSum += offer.RequirementsCost * request.Count;

        context.Response.ProfileChanges[profile.Pmc._id].TradersData[request.TraderId] = new TraderData
        {
            _salesSum = traderInfo.salesSum
        };

        return Task.CompletedTask;
    }
}