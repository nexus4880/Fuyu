using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Common.Backend.Networking;

namespace Fuyu.Backend.EFTMain.Networking;

public abstract class AbstractEftHttpController : AbstractHttpController
{
    protected AbstractEftHttpController(Regex pattern) : base(pattern)
    {
        // match dynamic paths
    }

    protected AbstractEftHttpController(string path) : base(path)
    {
        // match static paths
    }

    public override Task RunAsync(HttpContext context)
    {
        var downcast = new EftHttpContext(context.Request, context.Response);
        return RunAsync(downcast);
    }

    public abstract Task RunAsync(EftHttpContext context);
}

public abstract class AbstractEftHttpController<TRequest> : AbstractEftHttpController where TRequest : class
{
    protected AbstractEftHttpController(Regex pattern) : base(pattern)
    {
        // match dynamic paths
    }

    protected AbstractEftHttpController(string path) : base(path)
    {
        // match static paths
    }

    public override Task RunAsync(EftHttpContext context)
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

    public abstract Task RunAsync(EftHttpContext context, TRequest body);
}