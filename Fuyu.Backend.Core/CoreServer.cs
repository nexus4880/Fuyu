using System.Collections.Generic;
using Fuyu.Backend.Core.Networking;
using Fuyu.Common.Backend.Networking;

namespace Fuyu.Backend.Core;

public class CoreServer : FuyuServer
{
    public CoreServer(IEnumerable<AbstractCoreHttpController> controllers) : base("core", 44300)
    {
        HttpRouter = new HttpRouter(controllers);
    }

    private void RegisterServices()
    {
        /*HttpRouter.AddController<PingController>();
        HttpRouter.AddController<AccountGameRegisterController>();
        HttpRouter.AddController<AccountGetController>();
        HttpRouter.AddController<AccountLoginController>();
        HttpRouter.AddController<AccountLogoutController>();
        HttpRouter.AddController<AccountRegisterController>();*/
    }
}