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
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection;
using Fuyu.Modding;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtools.GenerateFleaMarketOffers";
    public override string Name { get; } = "Fuyu.Devtools.GenerateFleaMarketOffers";

    private Thread _generateOffersThread;

    public override Task OnLoad(DependencyContainer container)
    {
        _generateOffersThread = new Thread(GenerateOffers);
        _generateOffersThread.Start();

        return Task.CompletedTask;
    }

    static void GenerateOffers()
    {
        var player = new RagfairPlayerUser(MongoId.Generate(), 344, EMemberCategory.Developer, EMemberCategory.Developer,
            "b1gdeveloper", 69.420f, true);
        Terminal.WriteLine("Generating offers...");

        var sw = Stopwatch.StartNew();
        var templates = EftOrm.Instance.GetItemTemplates()["data"]!.ToObject<Dictionary<MongoId, ItemTemplate>>();
        var handbook = EftOrm.Instance.GetHandbook();
        var success = 0;
        var failed = 0;

        foreach (var (tid, template) in templates)
        {
            if (template.Type == ENodeType.Node)
            {
                continue;
            }

            int price = HandbookService.Instance.GetPrice(tid, 100).Value;

            try
            {
                var items = ItemFactoryService.Instance.CreateItem(template);

                if (items[0].Updatable == null)
                {
                    items[0].Updatable = new ItemUpdatable();
                }

                items[0].Updatable.StackObjectsCount = Random.Shared.Next(100, 100000);

                var createdOffer = RagfairService.Instance.CreateAndAddOffer(
                    user: player,
                    items: items,
                    isBatch: false,
                    requirements: [
                        new HandoverRequirement() { Count = price, TemplateId = "5449016a4bdc2d6f028b456f" }
                    ],
                    lifetime: TimeSpan.FromHours(30d),
                    unlimitedCount: true
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