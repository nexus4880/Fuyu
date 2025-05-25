using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection;
using Fuyu.Modding;

namespace Fuyu.Devtools.GenerateFleaMarketWeapons;

public class Mod : AbstractMod
{
    public override string Id => "Fuyu.Devtool.GenerateFleaMarketWeapons";

    public override string Name => "Fuyu.GenerateFleaMarketWeapons";

    private EftOrm _eftOrm;

    private ItemFactoryService _itemFactoryService;

    private HandbookService _handbookService;

    private RagfairService _ragfairService;

    private ItemFactoryOrm _itemFactoryOrm;

    private Thread _generateOffersThread;

    public override Task OnLoad(DependencyContainer container)
    {
        _eftOrm = EftOrm.Instance;
        _itemFactoryService = ItemFactoryService.Instance;
        _handbookService = HandbookService.Instance;
        _ragfairService = RagfairService.Instance;
        _itemFactoryOrm = ItemFactoryOrm.Instance;

        _generateOffersThread = new Thread(GenerateOffers)
        {
            // This thread will not keep the application alive
            IsBackground = true
        };

        _generateOffersThread.Start();

        return Task.CompletedTask;
    }

    private List<ItemInstance> CreateItemAndFillSlots(ItemTemplate template, string parent, string slotId)
    {
        var items = new List<ItemInstance>();
        var createdItems = _itemFactoryService.CreateItem(template);

        items.AddRange(createdItems);

        var rootItem = createdItems[0];

        rootItem.ParentId = parent;
        rootItem.SlotId = slotId;

        var weaponProperties = _itemFactoryService.GetItemProperties<WeaponItemProperties>(template);
        var handbook = _eftOrm.GetHandbook();

        foreach (var slot in weaponProperties.Slots)
        {
            var slotPropertyFilters = slot.Properties.Filters;

            if (slotPropertyFilters.Count == 0)
            {
                continue;
            }

            var itemFilters = slotPropertyFilters[0].Filter.FindAll(f => handbook.Items.Exists(hi => hi.Id == f)).ToArray();

            if (itemFilters.Length == 0)
            {
                continue;
            }

            var subTemplateId = itemFilters[Random.Shared.Next(0, itemFilters.Length)];
            var subTemplate = _itemFactoryOrm.GetItemTemplate(subTemplateId);
            var subItems = CreateItemAndFillSlots(subTemplate, rootItem.Id, slot.Name);

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
                var weaponTemplate = _itemFactoryOrm.GetItemTemplate(weapon.Id);
                var weaponItemStack = CreateItemAndFillSlots(weaponTemplate, "hideout", "hideout");

                if (weaponItemStack[0].Updatable == null)
                {
                    weaponItemStack[0].Updatable = new ItemUpdatable();
                }

                var upd = weaponItemStack[0].Updatable;

                var createdOffer = _ragfairService.CreateAndAddOffer(
                    user: user,
                    items: weaponItemStack,
                    isBatch: false,
                    quantity: 100000,
                    requirements: [
                        new HandoverRequirement
                        {
                            TemplateId = "5449016a4bdc2d6f028b456f",
                            Count = 100
                        }
                    ],
                    lifetime: TimeSpan.FromDays(1d)
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
            catch (Exception)
            {
                failed++;
            }
        }

        Terminal.WriteLine($"Done generating weapons: {sw.ElapsedMilliseconds}ms, {created} succeeded and {failed} failed");
    }
}