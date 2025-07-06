using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class FriendListController : AbstractEftHttpController
{
    public FriendListController() : base("/client/friend/list")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<FriendListResponse>()
        {
            data = new FriendListResponse()
            {
                Friends = [],
                Ignore = [],
                InIgnoreList = []
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}