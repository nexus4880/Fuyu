using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class LanguagesController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public LanguagesController(IGameDataRepository gameData) : base("/client/languages")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var languages = await _gameData.GetLanguagesAsync();
        var response = new ResponseBody<Dictionary<string, string>>
        {
            data = languages
        };

        await context.SendResponseAsync(response, true, true);
    }
}