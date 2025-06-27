using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class CheckVersionController : AbstractEftHttpController
{
    public CheckVersionController() : base("/client/checkVersion")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: Add global constant somewhere where we can define the supported version of EFT/Arena?
        // -- slejmur, 2025-01-09
        var currentVersion = "0.16.7.1.37759";
        var appVersion = context.EftVersion.Replace("EFT Client ", string.Empty);

        var response = new ResponseBody<CheckVersionResponse>()
        {
            data = new CheckVersionResponse()
            {
                IsValid = currentVersion == appVersion,
                //LatestVersion = currentVersion
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}