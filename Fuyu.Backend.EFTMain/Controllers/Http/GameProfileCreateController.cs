using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

// TODO:
// * move code into TemplateTable and ProfileService
// -- seionmoya, 2024/09/02
public class GameProfileCreateController : AbstractEftHttpController<GameProfileCreateRequest>
{
    private readonly EftOrm _eftOrm;
    private readonly ProfileService _profileService;

    public GameProfileCreateController() : base("/client/game/profile/create")
    {
        _eftOrm = EftOrm.Instance;
        _profileService = ProfileService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, GameProfileCreateRequest request)
    {
        var sessionId = context.SessionId;
        var account = _eftOrm.GetAccount(sessionId);
        var pmcId = _profileService.WipeProfile(account, request.side, request.headId, request.voiceId);

        var response = new ResponseBody<GameProfileCreateResponse>()
        {
            data = new GameProfileCreateResponse()
            {
                uid = pmcId
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}