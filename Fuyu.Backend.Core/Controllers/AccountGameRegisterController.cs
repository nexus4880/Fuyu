using System.Threading.Tasks;
using Fuyu.Backend.Core.Models.Requests;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.Core.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.Core.Controllers;

public class AccountGameRegisterController : AbstractCoreHttpController<AccountGameRegisterRequest>
{
    private readonly AccountService _accountService;

    public AccountGameRegisterController() : base("/account/game/register")
    {
        _accountService = AccountService.Instance;
    }

    public override Task RunAsync(CoreHttpContext context, AccountGameRegisterRequest request)
    {
        var sessionId = context.SessionId;
        var result = _accountService.RegisterGame(sessionId, request.Game, request.Edition);

        var text = Json.Stringify(result);
        return context.SendJsonAsync(text);
    }
}