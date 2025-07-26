using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Fuyu.Common.Serialization;
using Microsoft.AspNetCore.Http;

namespace Fuyu.Common.Backend.Networking;

public class HttpContext : WebRouterContext
{
    public HttpContext(HttpRequest request, HttpResponse response) : base(request, response)
    {
    }

    public virtual Task<byte[]> GetBinaryAsync()
    {
        using (var ms = new MemoryStream())
        {
            Request.Body.CopyTo(ms);
            return Task.FromResult(ms.ToArray());
        }
    }

    public virtual async Task<string> GetTextAsync()
    {
        var body = await GetBinaryAsync();
        return Encoding.UTF8.GetString(body);
    }

    public virtual async Task<T> GetJsonAsync<T>()
    {
        var json = await GetTextAsync();
        return Json.Parse<T>(json);
    }

    protected virtual Task SendAsync(byte[] data, string mime, HttpStatusCode status)
    {
        var hasData = !(data == null);

        Response.StatusCode = (int)status;
        Response.ContentType = mime;
        Response.ContentLength = hasData ? data.Length : 0;

        if (hasData)
        {
            using (var payload = Response.Body)
            {
                return payload.WriteAsync(data, 0, data.Length);
            }
        }
        else
        {
            Response.Body.Close();
            return Task.CompletedTask;
        }
    }

    public virtual Task SendStatus(HttpStatusCode status)
    {
        return SendAsync(null, "plain/text", status);
    }

    public virtual Task SendBinaryAsync(byte[] data, string mime)
    {
        return SendAsync(data, mime, HttpStatusCode.OK);
    }

    public virtual Task SendJsonAsync(string text, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var encoded = Encoding.UTF8.GetBytes(text);
        return SendAsync(encoded, "application/json; charset=utf-8", statusCode);
    }
}