using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class NotifierChannelCreateController : AbstractEftHttpController
{
    public static ThreadDictionary<string, string> Channels { get; } = new ThreadDictionary<string, string>();

    public NotifierChannelCreateController() : base("/client/notifier/channel/create")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: don't hardcode address
        // --seionmoya, 2024-11-18
        var address = "localhost:44301";
        var channelId = SimpleId.Generate(64);
        Channels.Set(channelId, context.SessionId);
        var response = new ResponseBody<NotifierChannelCreateResponse>
        {
            data = new NotifierChannelCreateResponse()
            {
                URL = $"https://{address}/push/notifier/get/{channelId}",
                WS = $"wss://{address}/push/notifier/getwebsocket/{channelId}"
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}