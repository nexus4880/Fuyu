using System.Threading.Tasks;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.Core.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.Core.Controllers;

public class AccountGetController : AbstractCoreHttpController
{
    private readonly AccountService _accountService;

    public AccountGetController(AccountService accountService) : base("/account/get")
    {
        _accountService = accountService;
    }

    public override async Task RunAsync(CoreHttpContext context)
    {
        var sessionId = context.SessionId;
        var response = await _accountService.GetStrippedAccountAsync(sessionId);

        var text = Json.Stringify(response);
        await context.SendJsonAsync(text);
    }
}