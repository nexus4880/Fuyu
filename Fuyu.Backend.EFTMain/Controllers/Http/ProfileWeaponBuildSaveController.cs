using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Templates;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ProfileWeaponBuildSaveController : AbstractEftHttpController<WeaponBuildSaveRequest>
{
    private readonly ResponseService _responseService;
    private readonly IProfileRepository _profiles;

    public ProfileWeaponBuildSaveController(IProfileRepository profiles) : base("/client/builds/weapon/save")
    {
        _responseService = ResponseService.Instance;
        _profiles = profiles;
    }

    public override async Task RunAsync(EftHttpContext context, WeaponBuildSaveRequest request)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var weaponBuild = profile.Builds.WeaponBuilds.Find(x => x.Id == request.Id);

        if (weaponBuild != null)
        {
            // Edit
            weaponBuild.Name = request.Name;
            weaponBuild.Items = request.Items;
            weaponBuild.Root = request.Root;
        }
        else
        {
            // Create
            weaponBuild = new WeaponBuild()
            {
                Id = request.Id,
                Name = request.Name,
                Root = request.Root,
                Items = request.Items
            };
        }

        profile.Builds.WeaponBuilds.Add(weaponBuild);

        await context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
    }
}