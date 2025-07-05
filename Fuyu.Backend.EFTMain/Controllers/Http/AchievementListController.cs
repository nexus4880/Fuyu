using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

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
        var text = response.ToString();

        await context.SendJsonAsync(text, true, true);
    }
}