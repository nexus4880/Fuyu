using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection;
using Fuyu.Modding;

namespace Fuyu.Devtools.GenerateFleaMarketOffers;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtool.GenerateFleaMarketOffers";

    public override string Name { get; } = "Fuyu-GenerateFleaMarketOffers";

    private HandbookService _handbookService;

    private ItemFactoryService _itemFactoryService;

    private RagfairService _ragfairService;

    private ItemFactoryOrm _itemFactoryOrm;

    private Thread _generateOffersThread;

    public override Task OnLoad(DependencyContainer container)
    {
        _handbookService = HandbookService.Instance;
        _itemFactoryService = ItemFactoryService.Instance;
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

    private void GenerateOffers()
    {
        var player = new RagfairPlayerUser(MongoId.Generate(), 301, EMemberCategory.Developer, EMemberCategory.Developer,
            "GenerateFleaMarketOffers", 1f, true);
        Terminal.WriteLine("Generating offers...");

        var sw = Stopwatch.StartNew();
        var templates = _itemFactoryOrm.GetItemTemplates();
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