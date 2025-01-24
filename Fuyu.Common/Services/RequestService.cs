using System;
using System.Collections.Generic;
using System.Text;
using Fuyu.Common.Networking;
using Fuyu.Common.Serialization;

namespace Fuyu.Common.Services;

public class RequestService
{
    public static RequestService Instance => instance.Value;
    private static readonly Lazy<RequestService> instance = new(() => new RequestService());

    private readonly Dictionary<string, HttpClient> _httpClients;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private RequestService()
    {
        _httpClients = [];
    }

    public void AddOrSetClient(string id, HttpClient httpClient)
    {
        if (_httpClients.ContainsKey(id))
        {
            _httpClients[id] = httpClient;
        }
        else
        {
            _httpClients.Add(id, httpClient);
        }
    }

    byte[] GetRequestBody(object o)
    {
        var json = Json.Stringify(o);
        var body = Encoding.UTF8.GetBytes(json);
        return body;
    }

    T GetResponseJson<T>(byte[] bytes)
    {
        var json = Encoding.UTF8.GetString(bytes);
        var o = Json.Parse<T>(json);
        return o;
    }

    public TResponse Get<TResponse>(string id, string path)
    {
        var resp = _httpClients[id].Get(path);
        return GetResponseJson<TResponse>(resp.Body);
    }

    public TResponse Post<TResponse>(string id, string path, object o)
    {
        var body = GetRequestBody(o);
        var resp = _httpClients[id].Post(path, body);
        return GetResponseJson<TResponse>(resp.Body);
    }

    public void Put(string id, string path, object o)
    {
        var body = GetRequestBody(o);
        _ = _httpClients[id].Put(path, body);
    }
}