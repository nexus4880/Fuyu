using System;
using System.Collections.Generic;
using Fuyu.Common.Client.Services;
using Fuyu.DependencyInjection;
using Fuyu.Modding;
using NLog.Targets;

[Target(nameof(FuyuClient))]
public sealed class FuyuClient : TargetWithLayout
{
    private FileSystemService _fileSystemService;
    private LogService _logService;

    protected override void InitializeTarget()
    {
        var container = new DependencyContainer();

        // resolve dependencies
        _fileSystemService = FileSystemService.Instance;
        _logService = LogService.Instance;

        // setup logging
        _logService.SetLogConfig("Fuyu.Client", "Fuyu/Logs/Client.log");
        AppDomain.CurrentDomain.UnhandledException += LogException;

        // verify game directory
        CheckIncompatibleSoftware();

        // load mods
        _logService.WriteLine("Loading mods...");
        ModManager.Instance.AddMods("./Fuyu/Mods/Client");
        ModManager.Instance.Load(container).GetAwaiter().GetResult();

        _logService.WriteLine("Finished loading!");
    }

    private void CheckIncompatibleSoftware()
    {
        var record = new Dictionary<string, string>()
        {
            { "BepInEx/", "BepInEx" },
            { "MelonLoader/", "MelonLoader" },
            { "SPT.Launcher/", "SPTarkov" },
            { "monomod.exe", "MonoMod" },
            { "MonoMod.RuntimeDetour.HookGen.exe", "MonoMod" }
            // TODO: UnityModManager, UMF
        };

        foreach (var kvp in record)
        {
            if (_fileSystemService.FileExists(kvp.Key))
            {
                var ex = new Exception($"{kvp.Value} found. Please remove the software from the client before proceeding.");
                _logService.WriteLine(ex);
                throw ex;
            }
        }
    }

    private void LogException(object sender, UnhandledExceptionEventArgs e)
    {
        _logService.WriteLine(e.ExceptionObject);
    }
}