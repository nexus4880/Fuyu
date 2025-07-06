using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileSelectController : AbstractEftHttpController
{
    public GameProfileSelectController() : base("/client/game/profile/select")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: handle this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<ProfileSelectResponse>()
        {
            data = new ProfileSelectResponse()
            {
                status = "ok"
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}