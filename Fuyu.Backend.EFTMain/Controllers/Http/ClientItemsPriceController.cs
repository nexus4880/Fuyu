using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public partial class ClientItemsPriceController : AbstractEftHttpController
{
    [GeneratedRegex(@"^/client/items/prices(/(?<traderId>[A-Za-z0-9]+))?$")]
    private static partial Regex PathExpression();

    private readonly EftOrm _eftOrm;
    private readonly HandbookService _handbook;

    public ClientItemsPriceController() : base(PathExpression())
    {
        _eftOrm = EftOrm.Instance;
        _handbook = HandbookService.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var parameters = context.GetPathParameters(this);
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var response = new ResponseBody<Union<SupplyData, Dictionary<MongoId, float>>>();

        if (parameters.TryGetValue("traderId", out var traderId))
        {
            // trader

            // NOTE: taken from dump client.items.prices
            // -- nexus4880, 2024-10-31
            var currencyCourses = new Dictionary<MongoId, double>
            {
                { "5449016a4bdc2d6f028b456f", 1d    },	// RUB
				{ "569668774bdc2da2298b4568", 144d  },	// EUR
				{ "5696686a4bdc2da3298b456a", 136d  },	// USD
				{ "5d235b4d86f7742e017bc88a", 7500d }	// GP Coin
			};

            response.data = new SupplyData
            {
                CurrencyCourses = currencyCourses,
                MarketPrices = profile.Pmc.Inventory.Items.DistinctBy(i => i.TemplateId).ToDictionary(i => i.TemplateId, i => (double)_handbook.GetPrice(i.TemplateId).Value),
                SupplyNextTime = (int)TimeSpan.FromSeconds(5d).Ticks
            };
        }
        else
        {
            // ragfair
            response.data = new Dictionary<MongoId, float>();
        }

        return context.SendResponseAsync(response, true, true);
    }
}