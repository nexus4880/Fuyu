using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Databases;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class TraderSettingsController : AbstractEftHttpController
{
    private readonly TraderDatabase _traderDatabase;

    public TraderSettingsController() : base("/client/trading/api/traderSettings")
    {
        _traderDatabase = TraderDatabase.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var response = new ResponseBody<IEnumerable<TraderTemplate>>
        {
            data = _traderDatabase.GetTraderTemplates().Values
        };

        return context.SendResponseAsync(response, true, true);
    }
}