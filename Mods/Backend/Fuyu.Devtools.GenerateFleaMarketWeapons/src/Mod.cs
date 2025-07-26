using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.BSG.Services;
using Fuyu.Common.Hashing;
using Fuyu.Modding;
using Microsoft.Extensions.Logging;

namespace Fuyu.Devtools.GenerateFleaMarketWeapons;

public class Mod : AbstractMod
{
    public override string Id => "Fuyu.Devtool.GenerateFleaMarketWeapons";

    public override string Name => "Fuyu.GenerateFleaMarketWeapons";

    private readonly IGameDataRepository _gameData;

    private readonly IItemTemplateRepository _itemTemplateRepository;

    private readonly ItemFactoryService _itemFactoryService;

    private readonly HandbookService _handbookService;

    private readonly RagfairService _ragfairService;

    private readonly ILogger<Mod> _logger;

    public Mod(
        ILogger<Mod> logger,
        HandbookService handbookService,
        RagfairService ragfairService,
        ItemFactoryService itemFactoryService,
        IItemTemplateRepository itemTemplateRepository,
        IGameDataRepository gameData)
    {
        _logger = logger;
        _gameData = gameData;
        _itemFactoryService = itemFactoryService;
        _handbookService = handbookService;
        _itemTemplateRepository = itemTemplateRepository;
        _ragfairService = ragfairService;
    }

    public override Task OnLoad()
    {
        return Task.Run(GenerateOffersAsync);
    }

    private async Task<List<ItemInstance>> CreateItemAndFillSlotsAsync(ItemTemplate template, string parent, string slotId)
    {
        var items = new List<ItemInstance>();
        var createdItems = await _itemFactoryService.CreateItemAsync(template);

        items.AddRange(createdItems);

        var rootItem = createdItems[0];

        rootItem.ParentId = parent;
        rootItem.SlotId = slotId;

        var weaponProperties = _itemFactoryService.GetItemProperties<WeaponItemProperties>(template);
        var handbook = await _gameData.GetHandbookAsync();

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
            var subTemplate = await _itemTemplateRepository.GetItemTemplateAsync(subTemplateId);
            var subItems = await CreateItemAndFillSlotsAsync(subTemplate, rootItem.Id, slot.Name);

            // Only happens when cancellation is requested, hence break
            if (subItems == null)
            {
                break;
            }

            items.AddRange(subItems);
        }

        return items;
    }

    private async Task GenerateOffersAsync()
    {
        var sw = Stopwatch.StartNew();
        var created = 0;
        var failed = 0;
        var weapons = await _handbookService.GetAllItemsOfTypeAsync("5b5f78dc86f77409407a7f8e");
        var user = new RagfairPlayerUser(MongoId.Generate(), 300, EMemberCategory.Developer, EMemberCategory.Developer, "GenerateFleaMarketWeapons", 1f, true);

        _logger.LogInformation("Generating weapons...");

        foreach (var weapon in weapons)
        {
            try
            {
                var weaponTemplate = await _itemTemplateRepository.GetItemTemplateAsync(weapon.Id);
                var weaponItemStack = await CreateItemAndFillSlotsAsync(weaponTemplate, "hideout", "hideout");

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

        _logger.LogInformation("Done generating weapons: {ElapsedMs}ms, {Created} succeeded and {Failed} failed", sw.ElapsedMilliseconds, created, failed);
    }
}