using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class PrestigeListController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public PrestigeListController() : base("/client/prestige/list")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2025-01-04
        var response = _eftOrm.GetPrestige();
        var text = response.ToString();
        return context.SendJsonAsync(text, true, true);
    }
}