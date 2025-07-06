using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GetMetricsConfigController : AbstractEftHttpController
{
    public GetMetricsConfigController() : base("/client/getMetricsConfig")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var response = new ResponseBody<GetMetricsConfigResponse>()
        {
            data = new GetMetricsConfigResponse()
            {
                Keys = [0, 8, 10, 13, 16, 20, 26, 30, 33, 45, 53, 66, 100, 500, 750, 1000],
                NetProcessingBins = [0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 13, 16, 20, 26, 30, 33, 45, 53, 66, 100, 500, 750, 1000],
                RenderBins = [0, 4, 8, 10, 13, 16, 20, 26, 30, 33, 45, 53, 66, 100, 500, 750, 1000],
                GameUpdateBins = [0, 4, 8, 10, 13, 16, 20, 26, 30, 33, 45, 53, 66, 100, 500, 750, 1000],
                MemoryMeasureInterval = 180,
                PauseReasons = [1, 2, 4, 7]
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}