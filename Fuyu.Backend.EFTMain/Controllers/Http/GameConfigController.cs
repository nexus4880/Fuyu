using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Servers;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameConfigController : AbstractEftHttpController
{
    public GameConfigController() : base("/client/game/config")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var response = new ResponseBody<GameConfigResponse>
        {
            data = new GameConfigResponse()
            {
                // TODO: don't use hardcoded path
                // --seionmoya, 2024-11-18
                backend = new Backends()
                {
                    Lobby = "https://localhost:44301",
                    Trading = "https://localhost:44301",
                    Messaging = "https://localhost:44301",
                    Main = "https://localhost:44301",
                    RagFair = "https://localhost:44301"
                },
                // TODO: update with TimeService later
                utc_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d,
                reportAvailable = true,
                // TODO: handle this
                // --seionmoya, 2024-11-18
                purchasedGames = new PurchasedGames()
                {
                    eft = true,
                    arena = true
                },
                // TODO: handle this
                // --seionmoya, 2024-11-18
                isGameSynced = true
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}