using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class LocationsController : AbstractEftHttpController
{
    // private readonly LocationService _locationService;
    private readonly EftOrm _eftOrm;

    public LocationsController() : base("/client/locations")
    {
        // _locationService = LocationService.Instance;
        _eftOrm = EftOrm.Instance;
    }

    // TODO: parse from model
    // -- seionmoya, 2024-01-09
    public override Task RunAsync(EftHttpContext context)
    {
        /*
        var worldmap = _locationService.GetWorldMap();
        var response = new ResponseBody<WorldMap>()
        {
            data = worldmap
        };
        
        */

        var response = _eftOrm.GetWorldMap();
        var text = response.ToString();
        return context.SendJsonAsync(text, true, true);
    }
}