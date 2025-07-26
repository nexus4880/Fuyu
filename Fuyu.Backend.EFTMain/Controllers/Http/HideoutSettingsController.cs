using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HideoutSettingsController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public HideoutSettingsController(IGameDataRepository gameData) : base("/client/hideout/settings")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var settings = await _gameData.GetHideoutSettingsAsync();
        var response = new ResponseBody<HideoutSettingsResponse>()
        {
            data = settings
        };

        await context.SendResponseAsync(response, true, true);
    }
}