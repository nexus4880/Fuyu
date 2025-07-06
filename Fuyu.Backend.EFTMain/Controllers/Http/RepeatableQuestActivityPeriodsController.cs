using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class RepeatableQuestActivityPeriodsController : AbstractEftHttpController
{
    public RepeatableQuestActivityPeriodsController() : base("/client/repeatalbeQuests/activityPeriods")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<object[]>
        {
            data = []
        };

        return context.SendResponseAsync(response, true, true);
    }
}