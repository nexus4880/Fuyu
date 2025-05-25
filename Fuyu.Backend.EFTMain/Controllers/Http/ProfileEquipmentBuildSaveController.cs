using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Templates;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ProfileEquipmentBuildSaveController : AbstractEftHttpController<EquipmentBuildSaveRequest>
{
    private readonly ResponseService _responseService;
    private readonly EftOrm _eftOrm;

    public ProfileEquipmentBuildSaveController() : base("/client/builds/equipment/save")
    {
        _responseService = ResponseService.Instance;
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, EquipmentBuildSaveRequest request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var equipmentBuild = profile.Builds.EquipmentBuilds.Find(x => x.Id == request.Id);

        if (equipmentBuild != null)
        {
            // Edit
            equipmentBuild.Name = request.Name;
            equipmentBuild.Root = request.Root;
            equipmentBuild.Items = request.Items;
        }
        else
        {
            // Create
            equipmentBuild = new EquipmentBuild()
            {
                Id = request.Id,
                Name = request.Name,
                Root = request.Root,
                Items = request.Items,
                BuildType = EEquipmentBuildType.Custom
            };
        }

        profile.Builds.EquipmentBuilds.Add(equipmentBuild);

        return context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
    }
}