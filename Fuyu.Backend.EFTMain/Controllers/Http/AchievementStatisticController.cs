using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class AchievementStatisticController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public AchievementStatisticController(IGameDataRepository gameData) : base("/client/achievement/statistic")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var statistics = await _gameData.GetAchievementStatisticsAsync();
        var response = new ResponseBody<AchievementStatisticResponse>()
        {
            data = statistics
        };

        await context.SendResponseAsync(response, true, true);
    }
}