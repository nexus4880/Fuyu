using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class BuildsListController : AbstractEftHttpController
{
    private readonly IProfileRepository _profiles;

    public BuildsListController(IProfileRepository profiles) : base("/client/builds/list")
    {
        _profiles = profiles;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var sessionId = context.SessionId;
        var profile = await _profiles.GetActiveProfileAsync(sessionId);
        var builds = profile.Builds;

        var response = new ResponseBody<BuildsListResponse>
        {
            data = builds
        };

        await context.SendResponseAsync(response, true, true);
    }
}