using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HideoutCustomizationOfferListController : AbstractEftHttpController
{
    private readonly IGameDataRepository _gameData;

    public HideoutCustomizationOfferListController(IGameDataRepository gameData) : base("/client/hideout/customization/offer/list")
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2025-01-04
        var response = await _gameData.GetHideoutCustomizationOffersAsync();
        await context.SendResponseAsync(new ResponseBody<JObject> { data = response }, true, true);
    }
}