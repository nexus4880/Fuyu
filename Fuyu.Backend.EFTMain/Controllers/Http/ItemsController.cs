using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ItemsController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public ItemsController(IGameDataRepository gameData) : base("/client/items")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = await _gameData.GetItemTemplatesAsync();
        var text = Json.Stringify(response);
        await context.SendJsonAsync(text, true, true);
    }
}