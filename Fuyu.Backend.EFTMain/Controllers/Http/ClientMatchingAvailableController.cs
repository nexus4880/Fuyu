using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientMatchingAvailableController : AbstractEftHttpController
{
    public ClientMatchingAvailableController() : base("/client/match/available")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var result = new ResponseBody<bool>
        {
            data = false
        };

        return context.SendResponseAsync(result, true, true);
    }
}