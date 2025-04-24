using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Fuyu.Common.Backend.Networking;

public class WsContext : WebRouterContext
{
    private const int _bufferSize = 32000;
    private readonly WebSocket _ws;

    public delegate Task OnTextEventHandler(WsContext sender, string text);
    public delegate Task OnBinaryEventHandler(WsContext sender, byte[] binary);
    public delegate Task OnCloseEventHandler(WsContext sender);

    public event OnTextEventHandler OnTextEvent;
    public event OnBinaryEventHandler OnBinaryEvent;
    public event OnCloseEventHandler OnCloseEvent;

    public WsContext(HttpRequest request, HttpResponse response, WebSocket ws) : base(request, response)
    {
        _ws = ws;
    }

    public bool IsOpen()
    {
        return _ws.State == WebSocketState.Open;
    }

    // TODO:
    // * use System.Buffers.ArrayPool for receiveBuffer
    // -- seionmoya, 2024/09/09

    // NOTE: Made this internal because consumers
    // shouldn't be calling this on their own
    // -- nexus4880, 2024-10-23
    internal async Task<bool> PollAsync()
    {
        var buffer = new byte[_bufferSize];
        WebSocketReceiveResult received;
        try
        {
            received = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        catch
        {
            return false;
        }

        var data = new byte[received.Count];
        Array.Copy(buffer, 0, data, 0, data.Length);

        switch (received.MessageType)
        {
            case WebSocketMessageType.Text:
                var text = Encoding.UTF8.GetString(data);
                if (OnTextEvent != null)
                {
                    await OnTextEvent(this, text);
                }
                break;

            case WebSocketMessageType.Binary:
                if (OnBinaryEvent != null)
                {
                    await OnBinaryEvent(this, data);
                }
                break;

            case WebSocketMessageType.Close:
                await CloseAsync();
                return false;
        }

        return true;
    }

    public Task SendTextAsync(string text)
    {
        var encoded = Encoding.UTF8.GetBytes(text);
        var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
        return _ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public Task SendBinaryAsync(byte[] data)
    {
        var buffer = new ArraySegment<byte>(data, 0, data.Length);
        return _ws.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public async Task CloseAsync()
    {
        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        if (OnCloseEvent != null)
        {
            await OnCloseEvent(this);
        }
    }

    public override string ToString()
    {
        return $"{GetType().Name}:{Path}";
    }
}