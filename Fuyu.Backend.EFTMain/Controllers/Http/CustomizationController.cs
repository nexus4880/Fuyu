using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class CustomizationController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public CustomizationController(IGameDataRepository gameData) : base("/client/customization")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var customizations = await _gameData.GetCustomizationsAsync();
        var response = new ResponseBody<Dictionary<string, CustomizationTemplate>>()
        {
            data = customizations
        };

        await context.SendResponseAsync(response, true, true);
    }
}