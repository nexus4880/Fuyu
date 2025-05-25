using System;
using System.IO;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Databases;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Loaders;

public class TraderLoader
{
    public static TraderLoader Instance => instance.Value;
    private static readonly Lazy<TraderLoader> instance = new(() => new TraderLoader());

    private readonly TraderDatabase _traderDatabase;

    public TraderLoader()
    {
        _traderDatabase = TraderDatabase.Instance;
    }

    public void Load()
    {
        var tradersJson = Resx.GetText("eft", "database.client.trading.api.traderSettings.json");
        var body = Json.Parse<ResponseBody<TraderTemplate[]>>(tradersJson);

        foreach (var traderTemplate in body.data)
        {
            _traderDatabase.SetTraderTemplate(traderTemplate.Id, traderTemplate);

            string assortJson;

            try
            {
                assortJson = Resx.GetText("eft",
                    $"database.client.trading.api.getTraderAssort.{traderTemplate.Id}.json");
            }
            catch (FileNotFoundException)
            {
                Terminal.WriteLine($"Failed to get assort for {traderTemplate.Id}");
                continue;
            }

            var traderAssort = Json.Parse<TraderAssort>(assortJson);
            _traderDatabase.SetTraderAssort(traderTemplate.Id, traderAssort);

            Terminal.WriteLine($"Got assort for {traderTemplate.Id}");
        }
    }
}