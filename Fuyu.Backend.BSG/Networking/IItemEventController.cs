using Fuyu.Common.Backend.Networking;

namespace Fuyu.Backend.BSG.Networking;

public interface IItemEventController : IRouterController<ItemEventContext>
{
    public string Action { get; }
}