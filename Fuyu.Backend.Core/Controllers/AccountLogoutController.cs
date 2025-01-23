using System.Net;
using System.Threading.Tasks;
using Fuyu.Backend.Core.Networking;

namespace Fuyu.Backend.Core.Controllers;

public class AccountLogoutController : AbstractCoreHttpController
{
    public AccountLogoutController() : base("/account/logout")
    {
    }

    public override Task RunAsync(CoreHttpContext context)
    {
        var sessionId = context.SessionId;
        CoreOrm.Instance.RemoveSession(sessionId);

        return context.SendStatus(HttpStatusCode.OK);
    }
}