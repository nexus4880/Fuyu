using Fuyu.Backend.Core.Controllers;
using Fuyu.Common.Networking;

namespace Fuyu.Backend.Core;

public class CoreServer : HttpServer
{
    public CoreServer() : base("core", "https://localhost:44300/")
    {
    }

    public void RegisterServices()
    {
        HttpRouter.AddController<PingController>();
        HttpRouter.AddController<AccountGameRegisterController>();
        HttpRouter.AddController<AccountGetController>();
        HttpRouter.AddController<AccountLoginController>();
        HttpRouter.AddController<AccountLogoutController>();
        HttpRouter.AddController<AccountRegisterController>();
    }
}