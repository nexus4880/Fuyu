using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileListController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public GameProfileListController() : base("/client/game/profile/list")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var sessionId = context.SessionId;
        var profile = _eftOrm.GetActiveProfile(sessionId);
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

        return context.SendResponseAsync(response, true, true);
    }
}