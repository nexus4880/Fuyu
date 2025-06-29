using System.Collections.Generic;
using Fuyu.Common.Backend.Networking;

namespace Fuyu.Backend.BSG.Networking;

public class ItemEventRouter : Router<IItemEventController, ItemEventContext>
{
    public ItemEventRouter(IEnumerable<IItemEventController> controllers) : base(controllers)
    {
    }
}