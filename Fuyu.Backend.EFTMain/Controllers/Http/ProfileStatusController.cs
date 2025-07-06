using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Multiplayer;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ProfileStatusController : AbstractEftHttpController
{
    private readonly IProfileRepository _profiles;

    public ProfileStatusController(IProfileRepository profiles) : base("/client/profile/status")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var sessionId = context.SessionId;

        var profile = await _profiles.GetActiveProfileAsync(sessionId);

        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<ProfileStatusResponse>()
        {
            data = new ProfileStatusResponse()
            {
                maxPveCountExceeded = false,
                profiles =
                [
                    new ProfileStatusInfo
                    {
                        profileid = profile.Pmc._id,
                        profileToken = null,
                        status = "Free",
                        sid = string.Empty,
                        ip = string.Empty,
                        port = 0
                    },
                    new ProfileStatusInfo
                    {
                        profileid = profile.Savage._id,
                        profileToken = null,
                        status = "Free",
                        sid = string.Empty,
                        ip = string.Empty,
                        port = 0
                    }
                ]
            }
        };

        await context.SendResponseAsync(response, true, true);
    }
}