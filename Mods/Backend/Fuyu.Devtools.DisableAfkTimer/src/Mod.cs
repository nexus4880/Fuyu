using System.Threading.Tasks;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Controllers.Http;
using Fuyu.Devtools.DisableAfkTimer.Controllers;
using Fuyu.Modding;

namespace Fuyu.Devtools.DisableAfkTimer;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtool.DisableAfkTimer";

    public override string Name { get; } = "Fuyu-DisableAfkTimer";

    private readonly EftMainServer _eftMainServer;

    public Mod(EftMainServer eftMainServer)
    {
        _eftMainServer = eftMainServer;
    }

    public override Task OnLoad()
    {
        var router = _eftMainServer.HttpRouter;
        router.ReplaceController<SettingsController, OverrideSettingsController>();

        return Task.CompletedTask;
    }
}