using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection;
using Fuyu.Modding;

namespace Fuyu.Backend;

public class GenerateFleaMarketWeaponsMod : AbstractMod
{
    public override string Id => "Fuyu.Devtool.GenerateFleaMarketWeapons";

    public override string Name => "Fuyu.GenerateFleaMarketWeapons";

    private EftOrm _eftOrm;

    private ItemFactoryService _itemFactoryService;

    private HandbookService _handbookService;

    private RagfairService _ragfairService;

    private Thread _generateOffersThread;

    public override Task OnLoad(DependencyContainer container)
    {
        _eftOrm = EftOrm.Instance;
        _itemFactoryService = ItemFactoryService.Instance;
        _handbookService = HandbookService.Instance;
        _ragfairService = RagfairService.Instance;
        _generateOffersThread = new Thread(GenerateOffers)
        {
            // This thread will not keep the application alive
            IsBackground = true
        };

        _generateOffersThread.Start();

        return Task.CompletedTask;
    }

    private List<ItemInstance> CreateItemAndFillSlots(ItemFactoryService itemFactoryService, ItemTemplate template, string parent, string slotId)
    {
        var items = new List<ItemInstance>();
        var createdItems = itemFactoryService.CreateItem(template);

        items.AddRange(createdItems);

        var rootItem = createdItems[0];
        
        rootItem.ParentId = parent;
        rootItem.SlotId = slotId;

        var weaponProperties = itemFactoryService.GetItemProperties<WeaponItemProperties>(template);
        var handbook = _eftOrm.GetHandbook();

        foreach (var slot in weaponProperties.Slots)
        {
            var slotPropertyFilters = slot.Properties.Filters;
            
            if (slotPropertyFilters.Count == 0)
            {
                continue;
            }

            var itemFilters = slotPropertyFilters[0].Filter.Where(f => handbook.Items.Exists(hi => hi.Id == f)).ToArray();

            if (itemFilters.Length == 0)
            {
                continue;
            }

            var subTemplateId = itemFilters[Random.Shared.Next(0, itemFilters.Length)];
            var subTemplate = itemFactoryService.ItemTemplates[subTemplateId];
            var subItems = CreateItemAndFillSlots(itemFactoryService, subTemplate, rootItem.Id, slot.Name);

            // Only happens when cancellation is requested, hence break
            if (subItems == null)
            {
                break;
            }
            
            items.AddRange(subItems);
        }

        return items;
    }

    private void GenerateOffers()
    {
        var sw = Stopwatch.StartNew();
        var created = 0;
        var failed = 0;
        var weapons = _handbookService.GetAllItemsOfType("5b5f78dc86f77409407a7f8e");
        var user = new RagfairPlayerUser(MongoId.Generate(), 300, EMemberCategory.Developer, EMemberCategory.Developer, "GenerateFleaMarketWeapons", 1f, true);

        Terminal.WriteLine($"Generating weapons...");

        foreach (var weapon in weapons)
        {
            try
            {
                var weaponTemplate = _itemFactoryService.ItemTemplates[weapon.Id];
                var weaponItemStack = CreateItemAndFillSlots(_itemFactoryService, weaponTemplate, "hideout", "hideout");

                if (weaponItemStack[0].Updatable == null)
                {
                    weaponItemStack[0].Updatable = new ItemUpdatable();
                }

                var upd = weaponItemStack[0].Updatable;

                upd.StackObjectsCount = 100000;

                var createdOffer = _ragfairService.CreateAndAddOffer(
                    user: user,
                    items: weaponItemStack,
                    isBatch: false,
                    requirements: [
                        new HandoverRequirement
                        {
                            TemplateId = "5449016a4bdc2d6f028b456f",
                            Count = 100
                        }
                    ],
                    lifetime: TimeSpan.FromDays(1d),
                    unlimitedCount: false
                );

                if (createdOffer == null)
                {
                    failed++;
                }
                else
                {
                    created++;
                }
            }
            catch (Exception ex)
            {
                failed++;
            }
        }

        Terminal.WriteLine($"Done generating weapons: {sw.ElapsedMilliseconds}ms, {created} succeeded and {failed} failed");
    }
}
