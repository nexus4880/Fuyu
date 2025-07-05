using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;

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
        var text = _locationService.GetLoot(location);
        return context.SendJsonAsync(text, true, true);
    }
}