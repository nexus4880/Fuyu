using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class TraderSettingsController : AbstractEftHttpController
{
    private readonly ITraderRepository _traders;

    public TraderSettingsController(ITraderRepository traders) : base("/client/trading/api/traderSettings")
    {
        _traders = traders;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var templates = await _traders.GetTraderTemplatesAsync();
        var response = new ResponseBody<IEnumerable<TraderTemplate>>
        {
            data = templates.Values
        };

        await context.SendResponseAsync(response, true, true);
    }
}