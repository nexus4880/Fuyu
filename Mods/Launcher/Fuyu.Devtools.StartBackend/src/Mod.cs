using System.Diagnostics;
using System.Threading.Tasks;
using Fuyu.DependencyInjection;
using Fuyu.Modding;

namespace Fuyu.Launcher.EFT;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtools.StartBackend";
    public override string Name { get; } = "Fuyu.Devtools.StartBackend";
    public override string[] Dependencies { get; } = [];

    public override Task OnLoad(DependencyContainer container)
    {
        var cwd = "../../../../../Fuyu.Backend/bin/Debug/net9.0/win-x64";
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = $"{cwd}/Fuyu.Backend.exe",
                WorkingDirectory = cwd
            }
        };

        process.Start();
        return Task.CompletedTask;
    }
}