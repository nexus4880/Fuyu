using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Fuyu.Backend.Core.Services;
using Microsoft.AspNetCore.Http;
using FuyuHttpContext = Fuyu.Common.Backend.Networking.HttpContext;

namespace Fuyu.Backend.Core.Networking;

public class CoreHttpContext : FuyuHttpContext
{
    public CoreHttpContext(HttpRequest request, HttpResponse response) : base(request, response)
    {
    }

    public override byte[] GetBinary()
    {
        using (var ms = new MemoryStream())
        {
            Request.Body.CopyTo(ms);

            var body = ms.ToArray();
            var encryption = Encryption;

            /*if (!string.IsNullOrWhiteSpace(encryption))
            {
                // Tarkov Never sends AES encrypted body as request.
                switch (encryption)
                {
                    case "aes":
                        //body = CryptographyService.DecrypWeatherControllertAes(body);
                        break;

                    default:
                        throw new InvalidDataException(encryption);
                }
            }*/

            return body;
        }
    }

    protected Task SendAsync(byte[] data, string mime, HttpStatusCode status, bool encrypted)
    {
        var hasData = !(data == null);

        // Used for postman debugging by Nexus4880
        // -- seionmoya, 2024-11-18
#if DEBUG
        if (Request.Headers.ContainsKey("X-Require-Plaintext"))
        {
            encrypted = false;
        }
#endif

        if (hasData && encrypted)
        {
            Response.Headers.Add("X-Encryption", "aes");
            data = CryptographyService.EncryptAes(data);
            encrypted = false;
        }

        return SendAsync(data, mime, status);
    }

    public Task SendBinaryAsync(byte[] data, string mime, bool encrypted)
    {
        return SendAsync(data, mime, HttpStatusCode.OK, encrypted);
    }

    public Task SendJsonAsync(string text, bool encrypted)
    {
        var encoded = Encoding.UTF8.GetBytes(text);
        var mime = encrypted
            ? "application/octet-stream"
            : "application/json; charset=utf-8";

        return SendAsync(encoded, mime, HttpStatusCode.OK, encrypted);
    }

    public string Encryption
    {
        get
        {
            return Request.Headers["X-Encryption"];
        }
    }

    public string SessionId
    {
        get
        {
            return Request.Cookies["Session"];
        }
    }
}