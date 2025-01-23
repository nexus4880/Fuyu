using System.Threading.Tasks;
using Fuyu.Backend.Core.Networking;
using Fuyu.Backend.Core.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.Core.Controllers;

public class AccountGetController : AbstractCoreHttpController
{
    private readonly AccountService _accountService;

    public AccountGetController() : base("/account/get")
    {
        _accountService = AccountService.Instance;
    }

    public override Task RunAsync(CoreHttpContext context)
    {
        var sessionId = context.SessionId;
        var response = _accountService.GetStrippedAccount(sessionId);

        var text = Json.Stringify(response);
        return context.SendJsonAsync(text);
    }
}