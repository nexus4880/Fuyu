using System;
using System.IO;
using System.Threading.Tasks;
using Fuyu.Client.Arena.Patches;
using Fuyu.Client.Arena.Utils;
using Fuyu.Common.Client.Reflection;
using Fuyu.DependencyInjection;
using Fuyu.Modding;
using Microsoft.Win32;

namespace Fuyu.Client.Arena;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Client.Arena";
    public override string Name { get; } = "Fuyu.Client.Arena";

    private readonly AbstractPatch[] _patches;

    public Mod()
    {
        _patches = [
            new BattlEyePatch(),
            new ConsistencyGeneralPatch()
        ];
    }

    public override Task OnLoad(DependencyContainer container)
    {
        // TODO: disable when running on HTTPS
        // -- seionmoya, 2024-11-19
        new ProtocolUtil().RemoveTransportPrefixes();

        foreach (var patch in _patches)
        {
            patch.Enable();
        }

        return Task.CompletedTask;
    }

    public override Task OnShutdown()
    {
        foreach (var patch in _patches)
        {
            patch.Disable();
        }

        return Task.CompletedTask;
    }

    public void ValidateGameCopy()
    {
        var registryPath = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkovArena";
        var registryEntry = Registry.LocalMachine.OpenSubKey(registryPath, false).GetValue("InstallLocation");
        var installationPath = string.Empty;

        if (registryEntry != null)
        {
            installationPath = registryEntry.ToString();
        }
        else
        {
            throw new Exception("Could not find Live EFT Arena installation directory. Please ensure you ran Live EFT Arena at least once on your machine.");
        }

        var paths = new FileSystemInfo[]
        {
            new DirectoryInfo(installationPath),
            new FileInfo(Path.Combine(installationPath, "BattlEye/BEClient_x64.dll")),
            new FileInfo(Path.Combine(installationPath, "BattlEye/BEService_x64.exe")),
            new FileInfo(Path.Combine(installationPath, "ConsistencyInfo")),
            new FileInfo(Path.Combine(installationPath, "EscapeFromTarkov_BE.exe")),
            new FileInfo(Path.Combine(installationPath, "Uninstall.exe")),
            new FileInfo(Path.Combine(installationPath, "UnityCrashHandler64.exe"))
        };

        foreach (var info in paths)
        {
            if (!File.Exists(info.FullName))
            {
                throw new Exception("The Live EFT Arena installation either does not exist or is damaged. Please validate the integrity of your installation in BsgLauncher");
            }
        }
    }
}