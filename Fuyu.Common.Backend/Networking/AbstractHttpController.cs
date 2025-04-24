using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fuyu.Common.Backend.Networking;

public abstract class AbstractHttpController : AbstractWebController<HttpContext>
{
    protected AbstractHttpController(Regex pattern) : base(pattern)
    {
        // match dynamic paths
    }

    protected AbstractHttpController(string path) : base(path)
    {
        // match static paths
    }
}

public abstract class HttpController<TRequest> : AbstractHttpController where TRequest : class
{
    protected HttpController(Regex pattern) : base(pattern)
    {
        // match dynamic paths
    }

    protected HttpController(string path) : base(path)
    {
        // match static paths
    }

    public override Task RunAsync(HttpContext context)
    {
        // TODO:
        // - Use better exception type
        // -- seionmoya, 2024-10-13
        if (!context.HasBody())
        {
            throw new Exception("Request does not contain body.");
        }

        var body = context.GetJson<TRequest>();

        // TODO:
        // - Use better exception type
        // -- seionmoya, 2024-10-13
        if (body == null)
        {
            throw new Exception("Body could not be parsed as TRequest.");
        }

        return RunAsync(context, body);
    }

    public abstract Task RunAsync(HttpContext context, TRequest body);
}