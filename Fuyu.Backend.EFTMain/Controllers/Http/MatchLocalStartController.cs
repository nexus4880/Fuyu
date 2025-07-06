using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class MatchLocalStartController : AbstractEftHttpController<MatchLocalStartRequest>
{
    private readonly LocationService _locationService;

    public MatchLocalStartController(LocationService locationService) : base("/client/match/local/start")
    {
        _locationService = locationService;
    }

    public override Task RunAsync(EftHttpContext context, MatchLocalStartRequest request)
    {
        var location = request.location;
        var response = _locationService.GetLoot(location);
        return context.SendResponseAsync(new ResponseBody<JObject> { data = response }, true, true);
    }
}