using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Backend.Models.Requests;
using Fuyu.Common.Backend.Models.Responses;
using Fuyu.Common.Serialization;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class FuyuGameLoginController : AbstractEftHttpController<FuyuGameLoginRequest>
{
    private readonly AccountService _accountService;
    private readonly ILogger<FuyuGameLoginController> _logger;

    public FuyuGameLoginController(ILogger<FuyuGameLoginController> logger, AccountService accountService) : base("/fuyu/game/login")
    {
        _logger = logger;
        _accountService = accountService;
    }

    public override async Task RunAsync(EftHttpContext context, FuyuGameLoginRequest request)
    {
        var sessionId = await _accountService.LoginAccount(request.AccountId);
        var response = new FuyuGameLoginResponse()
        {
            SessionId = sessionId
        };

        _logger.LogInformation("Created session ID {SessionId}", sessionId);
        var text = Json.Stringify(response);
        // NOTE: no need for encryption, request runs internal
        // -- seionmoya, 2024-11-18
        await context.SendJsonAsync(text, false, false);
    }
}