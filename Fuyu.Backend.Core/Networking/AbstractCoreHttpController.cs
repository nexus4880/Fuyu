using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Common.Backend.Networking;

namespace Fuyu.Backend.Core.Networking;

public abstract class AbstractCoreHttpController : AbstractHttpController
{
    protected AbstractCoreHttpController(Regex pattern) : base(pattern)
    {
        // match dynamic paths
    }

    protected AbstractCoreHttpController(string path) : base(path)
    {
        // match static paths
    }

    public override Task RunAsync(HttpContext context)
    {
        var downcast = new CoreHttpContext(context.Request, context.Response);
        return RunAsync(downcast);
    }

    public abstract Task RunAsync(CoreHttpContext context);
}

public abstract class AbstractCoreHttpController<TRequest> : AbstractCoreHttpController where TRequest : class
{
    protected AbstractCoreHttpController(Regex pattern) : base(pattern)
    {
        // match dynamic paths
    }

    protected AbstractCoreHttpController(string path) : base(path)
    {
        // match static paths
    }

    public override async Task RunAsync(CoreHttpContext context)
    {
        if (!context.HasBody())
        {
            throw new RequestNoBodyException("Request does not contain body.");
        }

        var body = await context.GetJsonAsync<TRequest>();

        if (body == null)
        {
            throw new RequestBodyNotParsableException("Body could not be parsed as TRequest.");
        }

        await RunAsync(context, body);
    }

    public abstract Task RunAsync(CoreHttpContext context, TRequest body);
}