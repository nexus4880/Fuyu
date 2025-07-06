using System.Collections.Generic;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.Backend.Networking;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain;

public class EftMainServer : FuyuServer
{
    public EftMainServer(ILogger<EftMainServer> logger, IEnumerable<AbstractEftHttpController> controllers) : base(logger, "eft-main", 44301)
    {
        HttpRouter = new HttpRouter(controllers);
    }
}