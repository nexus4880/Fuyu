using System.Threading.Tasks;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Controllers.Http;
using Fuyu.Common.Backend.Networking;
using Fuyu.DependencyInjection;
using Fuyu.Devtools.DisableAfkTimer.Controllers;
using Fuyu.Modding;

namespace Fuyu.Devtools.DisableAfkTimer;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtool.DisableAfkTimer";

    public override string Name { get; } = "Fuyu-DisableAfkTimer";

    public override Task OnLoad(DependencyContainer container)
    {
        var eftMainServer = container.Resolve<FuyuServer, EftMainServer>();
        var router = eftMainServer.HttpRouter;
        router.ReplaceController<SettingsController, OverrideSettingsController>();

        return Task.CompletedTask;
    }
}