using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HideoutQteListController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public HideoutQteListController(IGameDataRepository gameData) : base("/client/hideout/qte/list")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = await _gameData.GetHideoutQtesAsync();
        await context.SendResponseAsync(new ResponseBody<JArray> { data = response }, true, true);
    }
}