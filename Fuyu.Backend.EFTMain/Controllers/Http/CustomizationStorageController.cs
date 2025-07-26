using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class CustomizationStorageController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public CustomizationStorageController(IGameDataRepository gameData) : base("/client/customization/storage")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var response = new ResponseBody<IEnumerable<CustomizationStorageEntry>>()
        {
            data = await _gameData.GetCustomizationStorageAsync()
        };

        await context.SendResponseAsync(response, true, true);
    }
}