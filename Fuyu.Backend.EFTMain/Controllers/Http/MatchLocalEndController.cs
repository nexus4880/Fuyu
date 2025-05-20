using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Raid;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class MatchLocalEndController : AbstractEftHttpController<MatchLocalEndRequest>
{
    private readonly EftOrm _eftOrm;
    private readonly ItemService _itemService;
    private readonly ResponseService _responseService;

    public MatchLocalEndController() : base("/client/match/local/end")
    {
        _eftOrm = EftOrm.Instance;
        _itemService = ItemService.Instance;
        _responseService = ResponseService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, MatchLocalEndRequest body)
    {
        var sessionId = context.SessionId;

        var profile = _eftOrm.GetActiveProfile(sessionId);
        var character = profile.Pmc._id == body.MatchEndResult.Profile._id ?
            profile.Pmc :
            profile.Savage;

        character.InsuredItems.RemoveAll(i => body.LostInsuredItems.Exists(j => j.Id == i.itemId));
        if (!body.MatchEndResult.ExitStatus.ShouldLoseItems())
        {
            var existingCounters = character.TaskConditionCounters;
            var shouldLoseFIR = body.MatchEndResult.ExitStatus.ShouldItemsLoseFIR();
            var newItems = new List<ItemInstance>(body.MatchEndResult.Profile.Inventory.ItemsMap.Count);
            foreach (var item in body.MatchEndResult.Profile.Inventory.Items)
            {
                // Also need to check time, ideally this would be done in a handler/service
                if (shouldLoseFIR)
                {
                    if (item.Updatable is not null)
                    {
                        item.Updatable.SpawnedInSession = false;
                    }
                }

                if (!character.Inventory.ItemsMap.TryAdd(item.Id, item))
                {
                    character.Inventory.ItemsMap[item.Id] = item;
                }
                else
                {
                    newItems.Add(item);
                }
            }

            // Does this code suck? Yes! Does it work? In my testing, yes!
            foreach (var newItem in newItems)
            {
                if (character.Inventory.ItemsMap.TryGetValue((MongoId)newItem.ParentId, out var parent))
                {
                    // If slot was previously occupied
                    var itemToReplace = character.Inventory.Items.Find(i => i.ParentId == newItem.ParentId && i.SlotId == newItem.SlotId);
                    if (itemToReplace is not null)
                    {
                        // If the item that was occupying the slot is no longer in their inventory
                        if (!body.MatchEndResult.Profile.Inventory.ItemsMap.ContainsKey(itemToReplace.Id))
                        {
                            // Remove the item from our inventory
                            character.Inventory.ItemsMap.Remove(itemToReplace.Id);
                        }
                        // else the item is somewhere else, don't remove it, we probably will update the position
                    }
                    // else the slot was empty
                }
                else
                {
                    // This shouldn't happen, leaving it here in case it does.
                    // I have yet to see it happen though so that's good.
                    // -- nexus4880, 2025-5-18
                    Terminal.WriteLine($"{newItem.Id}'s parent {newItem.ParentId} is not in ItemsMap!");
                }
            }
        }
        else
        {
            // TODO: Redo this later, it's wrong.
            List<MongoId> securedItems = [];

            var inventory = body.MatchEndResult.Profile.Inventory;
            var rootEquipmentItem = inventory.Items.Find(i => i.Id == inventory.Equipment);
            var equipmentItems = _itemService.GetItemAndChildren(inventory.Items, rootEquipmentItem);

            securedItems.Add(rootEquipmentItem.Id);

            var securedContainer = equipmentItems.Find(
                i => i.ParentId == inventory.Equipment && i.SlotId == "SecuredContainer"
            );

            if (securedContainer is not null)
            {
                securedItems.AddRange(
                    _itemService.GetItemAndChildren(inventory.Items, securedContainer)
                                .Select(i => i.Id)
                );
            }

            var armband = equipmentItems.Find(
                i => i.ParentId == inventory.Equipment && i.SlotId == "ArmBand"
            );

            if (armband is not null)
            {
                securedItems.Add(armband.Id);
            }

            var dogtag = equipmentItems.Find(
                i => i.ParentId == inventory.Equipment && i.SlotId == "Dogtag"
            );

            if (dogtag is not null)
            {
                securedItems.Add(dogtag.Id);
            }

            var scabbard = equipmentItems.Find(
                i => i.ParentId == inventory.Equipment && i.SlotId == "Scabbard"
            );

            if (scabbard is not null)
            {
                securedItems.Add(scabbard.Id);
            }

            var pockets = equipmentItems.Find(
                i => i.ParentId == inventory.Equipment && i.SlotId == "Pockets"
            );

            if (pockets is not null)
            {
                securedItems.Add(pockets.Id);
                securedItems.AddRange(
                    equipmentItems.Where(i => i.ParentId == pockets.Id &&
                                              i.SlotId == "SpecialSlot")
                                  .Select(i => i.Id)
                );

                /*securedItems.AddRange(
                    _itemService.GetItemAndChildren(inventory.Items, pockets)
                                .Select(i => i.Id)
                );*/
            }

            foreach (var equipmentItem in equipmentItems)
            {
                if (securedItems.Contains(equipmentItem.Id))
                {
                    continue;
                }

                if (character.Inventory.ItemsMap.Remove(equipmentItem.Id))
                {
                    Terminal.WriteLine($"Removed {equipmentItem.Id}");
                }
                else
                {
                    Terminal.WriteLine($"Couldn't remove {equipmentItem.Id}, it must be a new item");
                }
            }
        }

        return context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
    }
}