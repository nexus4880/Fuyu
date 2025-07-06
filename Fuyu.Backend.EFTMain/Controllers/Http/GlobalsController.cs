using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GlobalsController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public GlobalsController(IGameDataRepository gameData) : base("/client/globals")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = await _gameData.GetGlobalsAsync();
        await context.SendResponseAsync(new ResponseBody<JObject> { data = response }, true, true);
    }
}