using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public partial class GetTraderAssortController : AbstractEftHttpController
{
    [GeneratedRegex("/client/trading/api/getTraderAssort/(?<traderId>[A-Za-z0-9]+)")]
    private static partial Regex PathExpression();

    private readonly EftOrm _eftOrm;
    private readonly TraderOrm _traderOrm;

    public GetTraderAssortController() : base(PathExpression())
    {
        _eftOrm = EftOrm.Instance;
        _traderOrm = TraderOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var parameters = context.GetPathParameters(this);
        var traderId = parameters["traderId"];
        var assort = _traderOrm.GetTraderAssort(traderId);

        if (assort == null)
        {
            throw new Exception($"Failed to find assort for trader {traderId}");
        }

        var profile = _eftOrm.GetActiveProfile(context.SessionId);

        if (!profile.Pmc.TradersInfo.HasValue)
        {
            throw new Exception("Player has no TradersInfo");
        }

        if (!profile.Pmc.TradersInfo.Value.IsValue1)
        {
            throw new Exception("TradersInfo is not Dictionary");
        }

        if (!profile.Pmc.TradersInfo.Value.Value1.TryGetValue(traderId, out var traderInfo))
        {
            throw new Exception($"User has no trader info for {traderId}");
        }

        var traderTemplate = _traderOrm.GetTraderTemplate(traderId);
        var assortClone = Json.Clone<TraderAssort>(assort);
        var level = 0;

        for (var index = 0; index < traderTemplate.LoyaltyLevels.Length; index++)
        {
            var loyaltyInfo = traderTemplate.LoyaltyLevels[index];

            if (loyaltyInfo.MinStanding < traderInfo.standing)
            {
                level = index + 1;
            }
        }

        foreach (var (id, requiredLevel) in assortClone.LoyaltyLevelItems)
        {
            if (requiredLevel > level)
            {
                assortClone.BarterScheme.Remove(id);
                var item = assortClone.Items.Find(i => i.Id == id);

                if (item != null)
                {
                    assortClone.Items.Remove(item);
                }
            }
        }

        var response = new ResponseBody<TraderAssort>
        {
            data = assortClone
        };

        return context.SendResponseAsync(response, true, true);
    }
}