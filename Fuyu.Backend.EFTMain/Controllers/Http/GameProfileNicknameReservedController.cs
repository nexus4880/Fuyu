using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileNicknameReservedController : AbstractEftHttpController
{
    private readonly IAccountRepository _accounts;

    public GameProfileNicknameReservedController(IAccountRepository accounts) : base("/client/game/profile/nickname/reserved")
    {
        _accounts = accounts;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var sessionId = context.SessionId;
        var account = await _accounts.GetBySessionAsync(sessionId);

        var response = new ResponseBody<string>()
        {
            data = account.Username
        };

        await context.SendResponseAsync(response, true, true);
    }
}