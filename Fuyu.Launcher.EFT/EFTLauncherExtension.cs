using System.IO;
using System.Threading.Tasks;
using Fuyu.Common.IO;
using Fuyu.Common.Launcher.Models.Settings;
using Fuyu.Common.Launcher.Services;
using Fuyu.Common.Networking;
using Fuyu.Common.Services;
using Fuyu.Launcher.EFT.Pages;

namespace Fuyu.Launcher.EFT;

public class EFTLauncherExtension
{
    private static readonly string _resourceId = "Fuyu.Launcher.EFT";

    private readonly ContentService _contentService;
    private readonly RequestService _requestService;
    private readonly SettingsService _settingsService;

    public EFTLauncherExtension(
        ContentService contentService,
        RequestService requestService,
        SettingsService settingsService)
    {
        _contentService = contentService;
        _requestService = requestService;
        _settingsService = settingsService;
    }

    public Task Initialize()
    {
        // Register resources
        Resx.SetSource(_resourceId, this.GetType().Assembly);

        // Load config
        EFTLauncherConfig.Instance.Load();
        var address = EFTLauncherConfig.Instance.EFTAddress;
        var gamepath = EFTLauncherConfig.Instance.GamePath;

        // Add launcher request client
        var eftHttpClient = new HttpClient(address);
        _requestService.AddOrSetClient("eft", eftHttpClient);

        var backendAddressSetting = new TextSetting()
        {
            Id = "address",
            Name = "Backend address",
            Description = "Game server address",
            Value = address
        };

        var gamePathSetting = new TextSetting()
        {
            Id = "gamepath",
            Name = "Game directory",
            Description = "Full path to the directory where EscapeFromTarkov.exe resides",
            Value = gamepath
        };

        backendAddressSetting.AddOnSaveCallback(OnSaveAddress);
        gamePathSetting.AddOnSaveCallback(OnSaveGamePath);

        // Add settings
        var settings = new SettingSection()
        {
            Id = "fuyu.launcher.eft",
            Name = "Escape From Tarkov",
            Settings = [
                backendAddressSetting,
                gamePathSetting
            ]
        };

        _settingsService.SetOrAddSection(settings);

        // Reguster pages
        InitializePages();
        InitializeAssets();

        return Task.CompletedTask;
    }

    void InitializePages()
    {
        _ = new GameEftPage();
    }

    void InitializeAssets()
    {
        //                              http://launcher.fuyu.api/* callback
        _contentService.SetOrAddLoader("assets/css/game-eft.css", LoadContent);
        _contentService.SetOrAddLoader("assets/img/bg-eft.png", LoadContent);
        _contentService.SetOrAddLoader("assets/img/logo-eft.png", LoadContent);
    }

    Stream LoadContent(string path)
    {
        return path switch
        {
            // filepath                    stream
            "assets/css/game-eft.css" => Resx.GetStream(_resourceId, "assets.css.game-eft.css"),
            "assets/img/bg-eft.png" => Resx.GetStream(_resourceId, "assets.img.bg-eft.png"),
            "assets/img/logo-eft.png" => Resx.GetStream(_resourceId, "assets.img.logo-eft.png"),
            _ => throw new FileNotFoundException()
        };
    }

    void OnSaveAddress(string value)
    {
        EFTLauncherConfig.Instance.EFTAddress = value;
        EFTLauncherConfig.Instance.Save();
    }

    void OnSaveGamePath(string value)
    {
        EFTLauncherConfig.Instance.GamePath = value;
        EFTLauncherConfig.Instance.Save();
    }
}