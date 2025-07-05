using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HideoutProductionRecipesController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public HideoutProductionRecipesController(IGameDataRepository gameData) : base("/client/hideout/production/recipes")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = await _gameData.GetHideoutProductionRecipesAsync();
        var text = response.ToString();
        await context.SendJsonAsync(text, true, true);
    }
}