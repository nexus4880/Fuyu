using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class LocationsController : AbstractEftHttpController
{
    // private readonly LocationService _locationService;
    private readonly IGameDataRepository _gameData;

    public LocationsController(IGameDataRepository gameData) : base("/client/locations")
    {
        // _locationService = LocationService.Instance;
        _gameData = gameData;
    }

    // TODO: parse from model
    // -- seionmoya, 2024-01-09
    public override async Task RunAsync(EftHttpContext context)
    {
        /*
        var worldmap = _locationService.GetWorldMap();
        var response = new ResponseBody<WorldMap>()
        {
            data = worldmap
        };
        
        */

        var response = await _gameData.GetWorldMapAsync();
        await context.SendResponseAsync(new ResponseBody<JObject> { data = response }, true, true);
    }
}