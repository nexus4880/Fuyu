using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class CustomizationStorageController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public CustomizationStorageController() : base("/client/customization/storage")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var response = new ResponseBody<CustomizationStorageEntry[]>()
        {
            data = _eftOrm.GetCustomizationStorage().ToArray()
        };

        return context.SendResponseAsync(response, true, true);
    }
}