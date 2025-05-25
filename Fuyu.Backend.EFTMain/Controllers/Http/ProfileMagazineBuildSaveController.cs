using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Templates;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ProfileMagazineBuildSaveController : AbstractEftHttpController<MagazineBuildSaveRequest>
{
    private readonly ResponseService _responseService;
    private readonly EftOrm _eftOrm;

    public ProfileMagazineBuildSaveController() : base("/client/builds/magazine/save")
    {
        _responseService = ResponseService.Instance;
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, MagazineBuildSaveRequest request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var magazineBuild = profile.Builds.MagazineBuilds.Find(x => x.Id == request.Id);

        if (magazineBuild != null)
        {
            // Edit
            magazineBuild.Name = request.Name;
            magazineBuild.TopCount = request.TopCount;
            magazineBuild.BottomCount = request.BottomCount;
            magazineBuild.Caliber = request.Caliber;
            magazineBuild.Items = request.Items;
        }
        else
        {
            // Create
            magazineBuild = new MagazineBuild()
            {
                Id = request.Id,
                Name = request.Name,
                TopCount = request.TopCount,
                BottomCount = request.BottomCount,
                Caliber = request.Caliber,
                Items = request.Items,
            };
        }

        profile.Builds.MagazineBuilds.Add(magazineBuild);

        return context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
    }
}