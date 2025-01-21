using System.IO;
using System.Threading.Tasks;
using Fuyu.Common.IO;
using Fuyu.Common.Networking;
using Fuyu.Common.Services;
using Fuyu.Common.Launcher.Models.Settings;
using Fuyu.Common.Launcher.Services;
using Fuyu.DependencyInjection;
using Fuyu.Launcher.EFT.Pages;
using Fuyu.Modding;


namespace Fuyu.Launcher.EFT;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Launcher.EFT";
    public override string Name { get; } = "Fuyu.Launcher.EFT";
    public override string[] Dependencies { get; } = [
        "Fuyu.Launcher.Core"
    ];

    private ContentService _contentService;
    private RequestService _requestService;
    private SettingsService _settingsService;

    public override Task OnLoad(DependencyContainer container)
    {
        // resolve dependencies
        _contentService = ContentService.Instance;
        _requestService = RequestService.Instance;
        _settingsService = SettingsService.Instance;

        // Register resources
        Resx.SetSource(Id, this.GetType().Assembly);

        // Add launcher request client
        var eftHttpClient = new HttpClient("http://localhost:8010");
        _requestService.AddOrSetClient("eft", eftHttpClient);

        // Add settings
        var settings = new SettingSection()
        {
            Id = "fuyu.launcher.eft",
            Name = "Escape From Tarkov",
            Settings = [
                new TextSetting()
                {
                    Id = "address",
                    Name = "Backend address",
                    Description = "Game server address",
                    Value = "http://localhost:8010"
                },
                new TextSetting()
                {
                    Id = "gamepath",
                    Name = "Game path",
                    Description = "Path to EscapeFromTarkov.exe",
                    Value = "C:/Games/EFT-Live/EscapeFromTarkov.exe"
                }
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
        _contentService.SetOrAddLoader("assets/css/game-eft.css",  LoadContent);
        _contentService.SetOrAddLoader("assets/img/bg-eft.png",    LoadContent);
        _contentService.SetOrAddLoader("assets/img/logo-eft.png",  LoadContent);
    }

    Stream LoadContent(string path)
    {
        return path switch
        {
            // filepath                    stream
            "assets/css/game-eft.css"   => Resx.GetStream(Id, "assets.css.game-eft.css"),
            "assets/img/bg-eft.png"     => Resx.GetStream(Id, "assets.img.bg-eft.png"),
            "assets/img/logo-eft.png"   => Resx.GetStream(Id, "assets.img.logo-eft.png"),
            _                           => throw new FileNotFoundException()
        };
    }
}