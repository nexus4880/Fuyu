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
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class MatchLocalEndController : AbstractEftHttpController<MatchLocalEndRequest>
{
    private readonly EftOrm _eftOrm;
    private readonly ResponseService _responseService;

    public MatchLocalEndController() : base("/client/match/local/end")
    {
        _eftOrm = EftOrm.Instance;
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
            foreach ((var id, var item) in body.MatchEndResult.Profile.Inventory.ItemsMap)
            {
                // Also need to check time, ideally this would be done in a handler/service
                if (shouldLoseFIR)
                {
                    if (item.Updatable is not null)
                    {
                        item.Updatable.SpawnedInSession = false;
                    }
                }

                // TryAdd returns false if the key is already present
                if (!character.Inventory.ItemsMap.TryAdd(id, item))
                {
                    // In which case we update the existing item
                    character.Inventory.ItemsMap[id] = item;
                }
                else
                {
                    newItems.Add(item);
                }
            }

            // Does this code suck? Yes! Does it work? In my testing, yes!
            foreach (var newItem in newItems)
            {
                var parent = character.Inventory.ItemsMap[newItem.ParentId];
                try
                {
                    var oldItem = character.Inventory.ItemsMap.First(i => i.Value.ParentId == newItem.ParentId && i.Value.SlotId == newItem.SlotId);
                    character.Inventory.ItemsMap.Remove(oldItem.Key);
                }
                catch (InvalidOperationException e)
                {
                    Terminal.WriteLine(e);
                }
            }
        }
        else
        {
            var items = character.Inventory.Items;
            var safeItems = new List<ItemInstance>();
            var equipmentItem = items.Find(i => i.Id == character.Inventory.Equipment);
            var equipmentItems = ItemService.Instance.GetItemAndChildren(items, equipmentItem);
            safeItems.Add(equipmentItem);

            var armband = equipmentItems.Find(i => i.SlotId == "ArmBand");
            if (armband is not null)
            {
                safeItems.Add(armband);
            }

            var dogtag = equipmentItems.Find(i => i.SlotId == "Dogtag");
            if (dogtag is not null)
            {
                safeItems.Add(dogtag);
            }

            var pockets = equipmentItems.Find(i => i.SlotId == "Pockets");
            if (pockets is not null)
            {
                safeItems.Add(pockets);
            }

            var securedContainer = equipmentItems.Find(i => i.SlotId == "SecuredContainer");
            if (securedContainer is not null)
            {
                safeItems.AddRange(ItemService.Instance.GetItemAndChildren(items, securedContainer));
            }

            var scabbard = equipmentItems.Find(i => i.SlotId == "Scabbard");
            if (scabbard is not null)
            {
                safeItems.Add(scabbard);
            }

            if (character.Inventory.Stash.HasValue)
            {
                var stashItem = items.Find(i => i.Id == character.Inventory.Stash.Value);
                safeItems.AddRange(ItemService.Instance.GetItemAndChildren(items, stashItem));
            }

            if (character.Inventory.QuestStashItems.HasValue)
            {
                var questStash = items.Find(i => i.Id == character.Inventory.QuestStashItems.Value);
                safeItems.AddRange(ItemService.Instance.GetItemAndChildren(items, questStash));
            }

            if (character.Inventory.SortingTable.HasValue)
            {
                var sortingTable = items.Find(i => i.Id == character.Inventory.SortingTable.Value);
                safeItems.AddRange(ItemService.Instance.GetItemAndChildren(items, sortingTable));
            }

            var itemsToRemove = character.Inventory.ItemsMap.Where(i => !safeItems.Contains(i.Value));
            foreach (var itemToRemove in itemsToRemove)
            {
                character.Inventory.ItemsMap.Remove(itemToRemove.Key);
            }
        }
        
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, blocking: false, compacting: false);

        return context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
    }
}