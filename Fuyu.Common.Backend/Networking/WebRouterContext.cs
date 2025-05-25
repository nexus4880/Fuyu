using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Fuyu.Common.Backend.Networking;

public class WebRouterContext : IRouterContext
{
    public readonly HttpRequest Request;
    public readonly HttpResponse Response;
    public string Path { get; }
    public string Host { get; }

    public WebRouterContext(HttpRequest request, HttpResponse response)
    {
        Request = request;
        Response = response;
        Path = Request.Path;
        Host = $"{Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{Request.HttpContext.Connection.RemotePort}";
    }

    public Dictionary<string, string> GetPathParameters(IRoutable routable)
    {
        var result = new Dictionary<string, string>();
        var match = routable.Matcher.Match(Path);

        if (match.Success)
        {
            var names = routable.Matcher.GetGroupNames();

            // NOTE: index 0 is always "0"
            // -- nexus4880, 2024-10-11
            for (int i = 1; i < names.Length; i++)
            {
                var groupName = names[i];
                result[groupName] = match.Groups[groupName].Value;
            }
        }

        return result;
    }

    public bool HasBody()
    {
        return Request.ContentLength.HasValue;
    }

    public void Close()
    {
        Response.Body.Close();
    }

    public override string ToString()
    {
        return $"{GetType().Name}:{Path}(HasBody:{HasBody()})";
    }
}