using System.Threading.Tasks;

namespace Fuyu.Common.Backend.Networking;

public interface IRouterController<TContext> where TContext : IRouterContext
{
    Task RunAsync(TContext context);
    bool IsMatch(TContext context);
}