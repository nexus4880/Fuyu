using System.Threading.Tasks;
using Fuyu.Backend.Core.Models.Requests;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.Core.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.Core.Controllers;

public class AccountGameRegisterController : AbstractCoreHttpController<AccountGameRegisterRequest>
{
    private readonly AccountService _accountService;

    public AccountGameRegisterController(AccountService accountService) : base("/account/game/register")
    {
        _accountService = accountService;
    }

    public override async Task RunAsync(CoreHttpContext context, AccountGameRegisterRequest request)
    {
        var sessionId = context.SessionId;
        var result = await _accountService.RegisterGameAsync(sessionId, request.Game, request.Edition);

        var text = Json.Stringify(result);
        await context.SendJsonAsync(text);
    }
}