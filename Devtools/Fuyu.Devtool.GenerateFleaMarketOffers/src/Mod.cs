using System;
using System.Collections.Generic;
using System.Diagnostics;
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

public class GenerateFleaMarketOffersMod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtool.GenerateFleaMarketOffers";

    public override string Name { get; } = "Fuyu-GenerateFleaMarketOffers";

    private HandbookService _handbookService;

    private EftOrm _eftOrm;

    private ItemFactoryService _itemFactoryService;

    private RagfairService _ragfairService;

    private Thread _generateOffersThread;

    public override Task OnLoad(DependencyContainer container)
    {
        _eftOrm = EftOrm.Instance;
        _handbookService = HandbookService.Instance;
        _itemFactoryService = ItemFactoryService.Instance;
        _ragfairService = RagfairService.Instance;
        _generateOffersThread = new Thread(GenerateOffers)
        {
            // This thread will not keep the application alive
            IsBackground = true
        };

        _generateOffersThread.Start();

        return Task.CompletedTask;
    }

    private void GenerateOffers()
    {
        var player = new RagfairPlayerUser(MongoId.Generate(), 301, EMemberCategory.Developer, EMemberCategory.Developer,
            "GenerateFleaMarketOffers", 1f, true);
        Terminal.WriteLine("Generating offers...");

        var sw = Stopwatch.StartNew();
        var templates = _eftOrm.GetItemTemplates()["data"]!.ToObject<Dictionary<MongoId, ItemTemplate>>();
        var handbook = _eftOrm.GetHandbook();
        var success = 0;
        var failed = 0;

        foreach (var (tid, template) in templates)
        {
            if (template.Type == ENodeType.Node)
            {
                continue;
            }

            int price = _handbookService.GetPrice(tid, 100).Value;

            try
            {
                var items = _itemFactoryService.CreateItem(template);

                items[0].Updatable ??= new ItemUpdatable();
                items[0].Updatable.StackObjectsCount = Random.Shared.Next(100, 100000);

                var createdOffer = _ragfairService.CreateAndAddOffer(
                    user: player,
                    items: items,
                    isBatch: false,
                    requirements: [
                        new HandoverRequirement
                        {
                            TemplateId = "5449016a4bdc2d6f028b456f",
                            Count = price
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
                    success++;
                }
            }
            catch (Exception ex)
            {
                failed++;
            }
        }

        Terminal.WriteLine(
            $"Done generating offers: {sw.ElapsedMilliseconds}ms, {success} succeeded and {failed} failed");
    }
}