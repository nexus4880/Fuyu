using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileVoiceChangeController : AbstractEftHttpController<GameProfileVoiceChangeRequest>
{
    private readonly IProfileRepository _profiles;

    public GameProfileVoiceChangeController(IProfileRepository profiles) : base("/client/game/profile/voice/change")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(EftHttpContext context, GameProfileVoiceChangeRequest body)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        profile.Pmc.Info.Voice = body.Voice;

        // TODO: Save profile

        var response = new ResponseBody<object>()
        {
            data = null
        };

        await context.SendResponseAsync(response, true, true);
    }
}