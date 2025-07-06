using System.Threading.Tasks;
using Fuyu.Backend.Core.Models.Requests;
using Fuyu.Backend.Core.Models.Responses;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.Core.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.Core.Controllers;

public class AccountRegisterController : AbstractCoreHttpController<AccountRegisterRequest>
{
    private readonly AccountService _accountService;

    public AccountRegisterController(AccountService accountService) : base("/account/register")
    {
        _accountService = accountService;
    }

    public override async Task RunAsync(CoreHttpContext context, AccountRegisterRequest request)
    {
        var result = await _accountService.RegisterAccountAsync(request.Username, request.Password);
        var response = new AccountRegisterResponse()
        {
            Status = result
        };

        var text = Json.Stringify(response);
        await context.SendJsonAsync(text);
    }
}