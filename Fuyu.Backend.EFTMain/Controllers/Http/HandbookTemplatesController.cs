using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HandbookTemplatesController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public HandbookTemplatesController(IGameDataRepository gameData) : base("/client/handbook/templates")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<HandbookTemplates>()
        {
            data = await _gameData.GetHandbookAsync()
        };

        await context.SendResponseAsync(response, true, true);
    }
}