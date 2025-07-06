using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class SettingsController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public SettingsController(IGameDataRepository gameData) : base("/client/settings")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = await _gameData.GetSettingsAsync();
        await context.SendResponseAsync(new ResponseBody<JObject> { data = response }, true, true);
    }
}