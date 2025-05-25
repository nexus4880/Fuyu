using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Servers;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ServerListController : AbstractEftHttpController
{
    public ServerListController() : base("/client/server/list")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // The client doesn't actually do anything from what I see, no need to do anything here
        // - nexus4880, 2025-5-17
        var response = new ResponseBody<ServerInfo[]>()
        {
            data = []
        };

        return context.SendResponseAsync(response, true, true);
    }
}