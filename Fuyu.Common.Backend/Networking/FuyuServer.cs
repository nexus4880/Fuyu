using System;
using System.Net;
using System.Threading.Tasks;
using Fuyu.Common.IO;
using AspNetHttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace Fuyu.Common.Backend.Networking;

public class FuyuServer
{
    public HttpRouter HttpRouter { get; protected set; }
    public WsRouter WsRouter { get; protected set; }

    public readonly int Port;
    public readonly string Name;
    public readonly string SubProtocol;

    public FuyuServer(string name, int port, string subprotocol = null)
    {
        Port = port;
        Name = name;
        SubProtocol = subprotocol;
    }

    public virtual Task OnRequestAsync(AspNetHttpContext ctx)
    {
        if (ctx.WebSockets.IsWebSocketRequest)
        {
            return OnWsRequestAsync(ctx);
        }
        else
        {
            return OnHttpRequestAsync(ctx);
        }
    }

    private async Task OnHttpRequestAsync(AspNetHttpContext ctx)
    {
        if (HttpRouter is null)
        {
            Terminal.WriteLine($"[{Name}] {nameof(HttpRouter)} is null, cannot handle {ctx.Request.Path}");
            return;
        }

        var context = new HttpContext(ctx.Request, ctx.Response);

        Terminal.WriteLine($"[{Name}][HTTP] {context.Path}");

        try
        {
            await HttpRouter.RouteAsync(context);
        }
        catch (RouteNotFoundException ex)
        {
            Terminal.WriteLine(ex.Message);
            await context.SendStatus(HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            Terminal.WriteLine(ex.Message);
            context.Close();
        }
    }

    private async Task OnWsRequestAsync(AspNetHttpContext ctx)
    {
        if (WsRouter is null)
        {
            Terminal.WriteLine($"[{Name}] {nameof(WsRouter)} is null, cannot handle {ctx.Request.Path}");
            return;
        }

        var ws = await ctx.WebSockets.AcceptWebSocketAsync(SubProtocol);

        try
        {
            var context = new WsContext(ctx.Request, ctx.Response, ctx.RequestAborted, ws);
            var time = DateTime.UtcNow.ToString();
            Terminal.WriteLine($"[{Name}][WS  ] {context.Path}");
            await WsRouter.RouteAsync(context);
        }
        catch (Exception ex)
        {
            Terminal.WriteLine(ex.Message);
            // NOTE: no need to manually close, websocket will be disposed
            // -- seionmoya, 2024/09/09 
        }
        finally
        {
            ws?.Dispose();
        }
    }
}