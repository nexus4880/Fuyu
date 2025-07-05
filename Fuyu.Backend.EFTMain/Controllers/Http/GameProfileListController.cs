using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileListController : AbstractEftHttpController
{
    private readonly IProfileRepository _profiles;

    public GameProfileListController(IProfileRepository profiles) : base("/client/game/profile/list")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var sessionId = context.SessionId;
        var profile = await _profiles.GetActiveProfileAsync(sessionId);
        Profile[] profiles;

        if (profile.ShouldWipe)
        {
            profiles = [];
        }
        else
        {
            profiles = [profile.Pmc, profile.Savage];
        }

        var response = new ResponseBody<Profile[]>()
        {
            data = profiles
        };

        await context.SendResponseAsync(response, true, true);
    }
}