using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Multiplayer;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ProfileStatusController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public ProfileStatusController() : base("/client/profile/status")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var sessionId = context.SessionId;

        var profile = _eftOrm.GetActiveProfile(sessionId);

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

        return context.SendResponseAsync(response, true, true);
    }
}