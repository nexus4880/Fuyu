using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fuyu.Common.Backend.Networking;

public abstract class AbstractWebController<TContext> : IRoutable, IRouterController<TContext> where TContext : WebRouterContext
{
    public Regex Matcher { get; }

    protected AbstractWebController(Regex pattern)
    {
        Matcher = pattern;
    }

    protected AbstractWebController(string path)
    {
        Matcher = new Regex($"^{path}$");
    }

    public bool IsMatch(TContext context)
    {
        return Matcher.IsMatch(context.Path);
    }

    public abstract Task RunAsync(TContext context);
}