using System.Threading.Tasks;
using Fuyu.Backend.Core.Models.Requests;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.Core.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.Core.Controllers;

public class AccountLoginController : AbstractCoreHttpController<AccountLoginRequest>
{
    private readonly AccountService _accountService;

    public AccountLoginController(AccountService accountService) : base("/account/login")
    {
        _accountService = accountService;
    }

    public override async Task RunAsync(CoreHttpContext context, AccountLoginRequest body)
    {
        var response = await _accountService.LoginAccountAsync(body.Username, body.Password);
        var text = Json.Stringify(response);
        await context.SendJsonAsync(text);
    }
}