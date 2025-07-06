using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class AchievementListController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public AchievementListController(IGameDataRepository gameData) : base("/client/achievement/list")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = await _gameData.GetAchievementsAsync();
        await context.SendResponseAsync(new ResponseBody<JObject> { data = response }, true, true);
    }
}