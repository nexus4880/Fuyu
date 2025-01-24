using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Backend.Models.Responses;
using Fuyu.Common.Models.Requests;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class FuyuGameRegisterController : AbstractEftHttpController<FuyuGameRegisterRequest>
{
    private readonly AccountService _accountService;

    public FuyuGameRegisterController() : base("/fuyu/game/register")
    {
        _accountService = AccountService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, FuyuGameRegisterRequest request)
    {
        var accountId = _accountService.RegisterAccount(request.Username, request.Edition);
        var response = new FuyuGameRegisterResponse()
        {
            AccountId = accountId
        };

        var text = Json.Stringify(response);
        // NOTE: no need for encryption, request runs internal
        // -- seionmoya, 2024-11-18
        return context.SendJsonAsync(text, false, false);
    }
}