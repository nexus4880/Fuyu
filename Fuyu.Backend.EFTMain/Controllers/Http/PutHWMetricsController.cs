using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class PutHWMetricsController : AbstractEftHttpController<HWMetricsRequest>
{
    public PutHWMetricsController() : base("/client/putHWMetrics")
    {
    }

    public override Task RunAsync(EftHttpContext context, HWMetricsRequest body)
    {
        return Task.CompletedTask;
    }
}