using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Common.Backend.Networking;
using Fuyu.Common.Compression;
using Fuyu.Common.Serialization;
using Microsoft.AspNetCore.Http;
using FuyuHttpContext = Fuyu.Common.Backend.Networking.HttpContext;

namespace Fuyu.Backend.EFTMain.Networking;

public class EftHttpContext : FuyuHttpContext
{
    public EftHttpContext(HttpRequest request, HttpResponse response) : base(request, response)
    {
    }

    public override byte[] GetBinary()
    {
        using (var ms = new MemoryStream())
        {
            Request.Body.CopyTo(ms);

            var body = ms.ToArray();
            var encryption = Encryption;

            if (!string.IsNullOrWhiteSpace(encryption))
            {
                switch (encryption)
                {
                    case "aes":
                        // TODO: handle AES-192 encryption
                        // body = CryptographyService.DecryptAes(body);
                        break;

                    default:
                        throw new InvalidDataException(encryption);
                }
            }

            if (MemoryZlib.IsCompressed(body))
            {
                body = MemoryZlib.Decompress(body);
            }

            return body;
        }
    }

    protected Task SendAsync(byte[] data, string mime, HttpStatusCode status, bool zipped, bool encrypted)
    {
        var hasData = !(data == null);

        // Used for postman debugging by Nexus4880
        // -- seionmoya, 2024-11-18
#if DEBUG
        if (Request.Headers.ContainsKey("X-Require-Plaintext"))
        {
            zipped = false;
            encrypted = false;
        }
#endif

        if (hasData && zipped)
        {
            data = MemoryZlib.Compress(data, CompressionLevel.SmallestSize);
        }

        if (hasData && encrypted)
        {
            // TODO: handle X-Encryption: aes
            /*
            Response.Headers.Add("X-Encryption", "aes");
            data = CryptographyService.EncryptAes(data);
            */
            encrypted = false;
        }

        return SendAsync(data, mime, status);
    }

    public Task SendBinaryAsync(byte[] data, string mime, bool zipped, bool encrypted)
    {
        return SendAsync(data, mime, HttpStatusCode.OK, zipped, encrypted);
    }

    public Task SendJsonAsync(string text, bool zipped, bool encrypted)
    {
        var encoded = Encoding.UTF8.GetBytes(text);
        var mime = zipped || encrypted
            ? "application/octet-stream"
            : "application/json; charset=utf-8";

        return SendAsync(encoded, mime, HttpStatusCode.OK, zipped, encrypted);
    }

    public Task SendResponseAsync<T>(ResponseBody<T> responseBody, bool zipped, bool encrypted)
    {
        var responseText = Json.Stringify(responseBody);
        return SendJsonAsync(responseText, zipped, encrypted);
    }

    public string Encryption
    {
        get
        {
            return Request.Headers["X-Encryption"];
        }
    }

    public string ETag
    {
        get
        {
            return Request.Headers["If-None-Match"];
        }
    }

    public string SessionId
    {
        get
        {
            return Request.Cookies["PHPSESSID"];
        }
    }

    /// <summary>
    /// Getting the current Eft version from the Request Headers.
    /// </summary>
    /// <returns>Client EFT Version</returns>
    public string EftVersion
    {
        // In header tarkov always sends the current version. (Example: "EFT Client 0.15.5.1.33420")

        // TODO: Replace the "EFT Client" or the same in Arena
        //  -- slejmur, 2025-01-09
        get
        {
            return Request.Headers["App-Version"];
        }
    }
}