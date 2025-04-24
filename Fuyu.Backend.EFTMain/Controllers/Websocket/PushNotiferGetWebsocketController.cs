using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Common.Backend.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Websocket;

public partial class PushNotiferGetWebsocketController : WsController
{
    public PushNotiferGetWebsocketController() : base(PathExpression())
    {
    }

    // NOTE: No event registrations as I am not implementing anything
    // -- nexus4880, 2024-10-23
    public override Task RunAsync(WsContext context)
    {
        return Task.CompletedTask;
    }

    [GeneratedRegex("^/push/notifier/getwebsocket/(?<channelId>[A-Za-z0-9]+)$")]
    private static partial Regex PathExpression();
}