using System.Collections.Generic;
using Fuyu.Backend.Core.Networking;
using Fuyu.Common.Backend.Networking;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.Core;

public class CoreServer : FuyuServer
{
    public CoreServer(ILogger<CoreServer> logger, IEnumerable<AbstractCoreHttpController> controllers) : base(logger, "core", 44300)
    {
        HttpRouter = new HttpRouter(controllers);
    }
}