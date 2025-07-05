using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Controllers.Http;
using Fuyu.Common.Backend.Networking;
using Fuyu.Common.Collections;

namespace Fuyu.Backend.EFTMain.Controllers.Websocket;

public partial class PushNotiferGetWebsocketController : WsController
{
    public static ThreadDictionary<string, WsContext> ActiveContexts { get; } = new ThreadDictionary<string, WsContext>();

    public PushNotiferGetWebsocketController() : base(PathExpression())
    {
    }

    public override Task RunAsync(WsContext context)
    {
        var parameters = context.GetPathParameters(this);
        if (!parameters.TryGetValue("channelId", out var channelId))
        {
            throw new Exception($"Invalid channelId {channelId ?? "not set"}");
        }

        if (!NotifierChannelCreateController.Channels.TryGet(channelId, out var sessionId))
        {
            throw new Exception("Received unexpected notifier connection");
        }

        NotifierChannelCreateController.Channels.Remove(channelId);
        ActiveContexts.Set(sessionId, context);

        return base.RunAsync(context);
    }

    public override Task OnCloseAsync(WsContext context)
    {
        var sessionId = ActiveContexts.ToDictionary().FirstOrDefault(c => c.Value == context).Key;
        if (sessionId is not null)
        {
            ActiveContexts.Remove(sessionId);
        }

        return base.OnCloseAsync(context);
    }

    [GeneratedRegex("^/push/notifier/getwebsocket/(?<channelId>[A-Za-z0-9]+)$")]
    private static partial Regex PathExpression();
}