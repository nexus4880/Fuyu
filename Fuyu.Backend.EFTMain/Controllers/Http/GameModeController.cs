using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameModeController : AbstractEftHttpController<ClientGameModeRequest>
{
    private readonly IAccountRepository _accounts;

    public GameModeController(IAccountRepository accounts) : base("/client/game/mode")
    {
        _accounts = accounts;
    }

    public override async Task RunAsync(EftHttpContext context, ClientGameModeRequest body)
    {
        var account = await _accounts.GetBySessionAsync(context.SessionId);

        if (body.SessionMode == null)
        {
            // wiped profile
            body.SessionMode = ESessionMode.Pve;
        }

        account.CurrentSession = body.SessionMode;

        var response = new ResponseBody<GameModeResponse>()
        {
            // TODO: don't use hardcoded address
            // --seionmoya, 2024-11-18
            data = new GameModeResponse()
            {
                GameMode = body.SessionMode,
                BackendUrl = "https://localhost:44301"
            }
        };

        await context.SendResponseAsync(response, true, true);
    }
}