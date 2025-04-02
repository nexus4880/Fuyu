using System;
using System.Net.Http;
using HttpClient = Fuyu.Common.Networking.HttpClient;

namespace Fuyu.Launcher.Core.Networking;

public class CoreHttpClient : HttpClient
{
    public readonly string _sessionId;

    public CoreHttpClient(string address, string sessionId) : base(address)
    {
        _sessionId = sessionId;
    }

    protected override HttpRequestMessage GetNewRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new Uri(Address + path),
        };

        request.Headers.Add("X-Encryption", "aes");
        request.Headers.Add("Cookie", $"Session={_sessionId}");

        return request;
    }
}