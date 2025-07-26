using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HideoutAreasController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public HideoutAreasController(IGameDataRepository gameData) : base("/client/hideout/areas")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = await _gameData.GetHideoutAreasAsync();
        await context.SendResponseAsync(new ResponseBody<JArray> { data = response }, true, true);
    }
}