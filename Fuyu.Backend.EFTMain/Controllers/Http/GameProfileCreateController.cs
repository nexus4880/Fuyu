using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Services;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

// TODO:
// * move code into TemplateTable and ProfileService
// -- seionmoya, 2024/09/02
public class GameProfileCreateController : AbstractEftHttpController<GameProfileCreateRequest>
{
    private readonly IAccountRepository _accounts;
    private readonly ProfileService _profileService;

    public GameProfileCreateController(IAccountRepository accounts, ProfileService profileService) : base("/client/game/profile/create")
    {
        _accounts = accounts;
        _profileService = profileService;
    }

    public override async Task RunAsync(EftHttpContext context, GameProfileCreateRequest request)
    {
        var sessionId = context.SessionId;
        var pmcId = await _profileService.WipeProfile(sessionId, request.side, request.headId, request.voiceId);

        var response = new ResponseBody<GameProfileCreateResponse>()
        {
            data = new GameProfileCreateResponse()
            {
                uid = pmcId
            }
        };

        await context.SendResponseAsync(response, true, true);
    }
}