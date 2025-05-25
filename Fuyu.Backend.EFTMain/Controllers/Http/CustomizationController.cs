using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class CustomizationController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public CustomizationController() : base("/client/customization")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var customizations = _eftOrm.GetCustomizations();
        var response = new ResponseBody<Dictionary<string, CustomizationTemplate>>()
        {
            data = customizations
        };

        return context.SendResponseAsync(response, true, true);
    }
}