using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Modding;

namespace Fuyu.Devtools.GenerateFleaMarketOffers;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtool.GenerateFleaMarketOffers";

    public override string Name { get; } = "Fuyu-GenerateFleaMarketOffers";

    private readonly IGameDataRepository _gameDataRepository;
    private readonly HandbookService _handbookService;

    private readonly ItemFactoryService _itemFactoryService;

    private readonly RagfairService _ragfairService;

    public Mod(
        IGameDataRepository gameDataRepository,
        HandbookService handbookService,
        ItemFactoryService itemFactoryService,
        RagfairService ragfairService
        )
    {
        _gameDataRepository = gameDataRepository;
        _handbookService = handbookService;
        _itemFactoryService = itemFactoryService;
        _ragfairService = ragfairService;
    }

    public override Task OnLoad()
    {
        return Task.Run(GenerateOffers);
    }

    private async Task GenerateOffers()
    {
        var player = new RagfairPlayerUser(MongoId.Generate(), 301, EMemberCategory.Developer, EMemberCategory.Developer,
            "GenerateFleaMarketOffers", 1f, true);
        Terminal.WriteLine("Generating offers...");

        var sw = Stopwatch.StartNew();
        var templates = await _gameDataRepository.GetItemTemplatesAsync();
        var success = 0;
        var failed = 0;

        foreach (var (tid, template) in templates)
        {
            if (template.Type == ENodeType.Node)
            {
                continue;
            }

            int price = (await _handbookService.GetPriceAsync(tid, 100)).Value;

            try
            {
                var items = await _itemFactoryService.CreateItemAsync(template);
                var count = Random.Shared.Next(100, 100000);
                var createdOffer = _ragfairService.CreateAndAddOffer(
                    user: player,
                    items: items,
                    quantity: count,
                    isBatch: false,
                    requirements: [
                        new HandoverRequirement
                        {
                            TemplateId = "5449016a4bdc2d6f028b456f",
                            Count = price
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
                    success++;
                }
            }
            catch (Exception)
            {
                failed++;
            }
        }

        Terminal.WriteLine(
            $"Done generating offers: {sw.ElapsedMilliseconds}ms, {success} succeeded and {failed} failed");
    }
}