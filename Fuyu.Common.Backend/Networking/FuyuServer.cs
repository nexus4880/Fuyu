using System;
using System.Net;
using System.Threading.Tasks;
using Fuyu.Common.Serialization;
using Microsoft.Extensions.Logging;
using AspNetHttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace Fuyu.Common.Backend.Networking;

public class FuyuServer
{
    public HttpRouter HttpRouter { get; protected set; }
    public WsRouter WsRouter { get; protected set; }

    private readonly ILogger _logger;
    public int Port { get; }
    public string Name { get; }
    public string SubProtocol { get; }
    public bool IsHTTPS { get; }

    public FuyuServer(ILogger logger, string name, int port, bool isHTTPS = true, string subprotocol = null)
    {
        _logger = logger;
        Port = port;
        Name = name;
        SubProtocol = subprotocol;
        IsHTTPS = isHTTPS;
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
            _logger.LogError("[{Name}] HttpRouter is null, cannot handle {ContextPath}", Name, ctx.Request.Path);
            return;
        }

        var context = new HttpContext(ctx.Request, ctx.Response);

        _logger.LogInformation("[{Name}][HTTP] {ContextPath}", Name, context.Path);

        try
        {
            await HttpRouter.RouteAsync(context);
        }
        catch (RouteNotFoundException ex)
        {
            _logger.LogError("{Message}", ex.Message);
            await context.SendStatus(HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            await context.SendJsonAsync(Json.Stringify(new { error = ex.Message }), HttpStatusCode.InternalServerError);
        }
    }

    private async Task OnWsRequestAsync(AspNetHttpContext ctx)
    {
        if (WsRouter is null)
        {
            _logger.LogError("[{Name}] WsRouter is null, cannot handle {ContextPath}", Name, ctx.Request.Path);
            return;
        }

        var ws = await ctx.WebSockets.AcceptWebSocketAsync(SubProtocol);

        try
        {
            var context = new WsContext(ctx.Request, ctx.Response, ctx.RequestAborted, ws);
            var time = DateTime.UtcNow.ToString();
            _logger.LogInformation("[{Name}][WS  ] {ContextPath}", Name, context.Path);
            await WsRouter.RouteAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            // NOTE: no need to manually close, websocket will be disposed
            // -- seionmoya, 2024/09/09 
        }
        finally
        {
            ws?.Dispose();
        }
    }
}