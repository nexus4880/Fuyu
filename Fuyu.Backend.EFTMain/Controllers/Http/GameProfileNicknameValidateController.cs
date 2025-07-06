using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileNicknameValidateController : AbstractEftHttpController<GameProfileNicknameValidateRequest>
{
    public GameProfileNicknameValidateController() : base("/client/game/profile/nickname/validate")
    {
    }

    public override Task RunAsync(EftHttpContext context, GameProfileNicknameValidateRequest request)
    {
        // TODO:
        // * validate nickname usage
        // -- seionmoya, 2024/08/28

        var response = new ResponseBody<GameProfileNicknameValidateResponse>()
        {
            data = new GameProfileNicknameValidateResponse()
            {
                status = "ok"
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}