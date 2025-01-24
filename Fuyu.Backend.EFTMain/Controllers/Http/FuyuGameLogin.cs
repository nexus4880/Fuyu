using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Backend.Models.Responses;
using Fuyu.Common.Models.Requests;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class FuyuGameLoginController : AbstractEftHttpController<FuyuGameLoginRequest>
{
    private readonly AccountService _accountService;

    public FuyuGameLoginController() : base("/fuyu/game/login")
    {
        _accountService = AccountService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, FuyuGameLoginRequest request)
    {
        var sessionId = _accountService.LoginAccount(request.AccountId);
        var response = new FuyuGameLoginResponse()
        {
            SessionId = sessionId
        };

        var text = Json.Stringify(response);
        // NOTE: no need for encryption, request runs internal
        // -- seionmoya, 2024-11-18
        return context.SendJsonAsync(text, false, false);
    }
}