using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain;

namespace Fuyu.Devtools.DisableAfkTimer.Controllers;

public class OverrideSettingsController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public OverrideSettingsController() : base("/client/settings")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var response = _eftOrm.GetSettings();

        // The client disables the timer if it's not positive
        response["data"]["config"]["AFKTimeoutSeconds"] = -1;

        var text = response.ToString();

        return context.SendJsonAsync(text, true, true);
    }
}