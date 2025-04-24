using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class NotifierChannelCreateController : AbstractEftHttpController
{
    public NotifierChannelCreateController() : base("/client/notifier/channel/create")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var channelId = SimpleId.Generate(64);

        // TODO: don't hardcode address
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<NotifierChannelCreateResponse>
        {
            data = new NotifierChannelCreateResponse()
            {
                Server = "localhost:44301",
                ChannelId = channelId,
                URL = $"https://localhost:44301/push/notifier/get/{channelId}",
                WS = $"wss://localhost:44301/push/notifier/getwebsocket/{channelId}"
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}