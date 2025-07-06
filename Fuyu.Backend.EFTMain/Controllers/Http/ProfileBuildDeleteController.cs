using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ProfileBuildDeleteController : AbstractEftHttpController<BuildDeleteRequest>
{
    private readonly IProfileRepository _profiles;
    private readonly ResponseService _responseService;

    public ProfileBuildDeleteController(IProfileRepository profiles) : base("/client/builds/delete")
    {
        _profiles = profiles;
        _responseService = ResponseService.Instance;
    }

    public override async Task RunAsync(EftHttpContext context, BuildDeleteRequest request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

        var index = profile.Builds.EquipmentBuilds.RemoveAll(x => x.Id == request.Id);
        if (index > 0)
        {
            goto completed;
        }

        index = profile.Builds.WeaponBuilds.RemoveAll(x => x.Id == request.Id);
        if (index > 0)
        {
            goto completed;
        }

        index = profile.Builds.MagazineBuilds.RemoveAll(x => x.Id == request.Id);
        if (index > 0)
        {
            goto completed;
        }

        throw new Exception($"Could not find a build with the id {request.Id}");

    completed:

        await context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
    }
}