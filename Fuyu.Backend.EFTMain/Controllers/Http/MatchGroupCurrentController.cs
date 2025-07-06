using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class MatchGroupCurrentController : AbstractEftHttpController
{
    public MatchGroupCurrentController() : base("/client/match/group/current")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: handle this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<MatchGroupCurrentResponse>()
        {
            data = new MatchGroupCurrentResponse()
            {
                squad = [],
                raidSettings = null
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}