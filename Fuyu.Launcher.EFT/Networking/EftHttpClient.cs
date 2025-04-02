using System;
using System.IO.Compression;
using System.Net.Http;
using Fuyu.Common.Compression;

namespace Fuyu.Launcher.EFT.Networking;

public class EftHttpClient : Fuyu.Common.Networking.HttpClient
{
    public readonly string _sessionId;

    public EftHttpClient(string address, string sessionId) : base(address)
    {
        _sessionId = sessionId;
    }

    protected override byte[] OnSendBody(byte[] body)
    {
        return MemoryZlib.Compress(body, CompressionLevel.SmallestSize);
    }

    protected override byte[] OnReceiveBody(byte[] body)
    {
        if (MemoryZlib.IsCompressed(body))
        {
            body = MemoryZlib.Decompress(body);
        }

        return body;
    }

    protected override HttpRequestMessage GetNewRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new Uri(Address + path),
        };

        request.Headers.Add("Cookie", $"PHPSESSID={_sessionId}");

        return request;
    }
}