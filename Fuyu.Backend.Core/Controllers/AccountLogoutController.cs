using System.Net;
using System.Threading.Tasks;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.Core.Controllers;

public class AccountLogoutController : AbstractCoreHttpController
{
    private readonly ICoreSessionRepository _sessions;

    public AccountLogoutController(ICoreSessionRepository sessions) : base("/account/logout")
    {
        _sessions = sessions;
    }

    public override async Task RunAsync(CoreHttpContext context)
    {
        await _sessions.RemoveAsync(context.SessionId);
        await context.SendStatus(HttpStatusCode.OK);
    }
}