using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Hashing;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientInsuranceItemsListCostController : AbstractEftHttpController<InsuranceCostRequest>
{
    private readonly EftOrm _eftOrm;

    public ClientInsuranceItemsListCostController() : base("/client/insurance/items/list/cost")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, InsuranceCostRequest body)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var items = body.ItemIds.Select(id => profile.Pmc.Inventory.ItemsMap[id]).ToArray();
        var response = new ResponseBody<InsuranceCostResponse>();

        if (items.Length == body.ItemIds.Length)
        {
            var insuranceCost = new InsuranceCostResponse(body.Traders.Length);
            var prices = new Dictionary<MongoId, int>(items.Length);

            foreach (var item in items)
            {
                prices[item.TemplateId] = 1;
            }

            foreach (var trader in body.Traders)
            {
                insuranceCost[trader] = prices;
            }

            response.data = insuranceCost;
        }
        else
        {
            response.errmsg = "One or more items could not be found on the backend";
        }

        return context.SendResponseAsync(response, true, true);
    }
}