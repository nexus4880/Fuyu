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
        var currentVersion = "0.16.1.3.35392";
        var appVersion = context.EftVersion;

        appVersion = appVersion.Replace("EFT Client ", "");

        var response = new ResponseBody<CheckVersionResponse>()
        {
            data = new CheckVersionResponse()
            {
                isvalid = false,
                latestVersion = currentVersion
            }
        };

        if (appVersion == currentVersion)
        {
            response.data.isvalid = true;
        }

        return context.SendResponseAsync(response, true, true);
    }
}