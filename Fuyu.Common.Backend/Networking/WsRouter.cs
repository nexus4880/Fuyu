using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fuyu.Common.Backend.Networking;

public class WsRouter : Router<WsController, WsContext>
{
    public WsRouter() : base()
    {
    }

    public WsRouter(IEnumerable<WsController> controllers) : base(controllers)
    {
    }

    public override async Task RouteAsync(WsContext context)
    {
        var matches = GetAllMatching(context);
        var tasks = new Task[matches.Count];
        for (var i = 0; i < matches.Count; i++)
        {
            tasks[i] = matches[i].RunAsync(context);
        }

        // Let them all initialize first
        await Task.WhenAll(tasks);

        while (await context.PollAsync())
        {
            // Reads from the connection
        }
    }
}