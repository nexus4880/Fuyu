using System.IO;
using System.Threading.Tasks;
using Fuyu.Common.IO;
using Fuyu.Common.Launcher.Services;
using Fuyu.Common.Networking;
using Fuyu.Common.Services;
using Fuyu.DependencyInjection;
using Fuyu.Launcher.Core.Pages;
using Fuyu.Modding;

namespace Fuyu.Launcher.Core;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Launcher.Core";
    public override string Name { get; } = "Fuyu.Launcher.Core";
    public override string[] Dependencies { get; } = [];

    private ContentService _contentService;
    private RequestService _requestService;

    public override Task OnLoad(DependencyContainer container)
    {
        // resolve dependencies
        _contentService = ContentService.Instance;
        _requestService = RequestService.Instance;

        Resx.SetSource(Id, this.GetType().Assembly);

        InitializePages();
        InitializeAssets();

        var coreHttpClient = new HttpClient("https://localhost:44300");
        _requestService.AddOrSetClient("core", coreHttpClient);

        return Task.CompletedTask;
    }

    void InitializePages()
    {
        _ = new IndexPage();
        _ = new AccountLibraryPage();
        _ = new AccountLoginPage();
        _ = new AccountRegisterPage();
        _ = new SettingsPage();
    }

    void InitializeAssets()
    {
        //                              http://launcher.fuyu.api/*      callback
        _contentService.SetOrAddLoader("assets/css/bootstrap.min.css", LoadContent);
        _contentService.SetOrAddLoader("assets/css/styles.css", LoadContent);
        _contentService.SetOrAddLoader("assets/js/bootstrap.min.js", LoadContent);
        _contentService.SetOrAddLoader("assets/js/popper.min.js", LoadContent);
    }

    Stream LoadContent(string path)
    {
        return path switch
        {
            // filepath                        stream
            "assets/css/bootstrap.min.css" => Resx.GetStream(Id, "assets.css.bootstrap.min.css"),
            "assets/css/styles.css" => Resx.GetStream(Id, "assets.css.styles.css"),
            "assets/js/bootstrap.min.js" => Resx.GetStream(Id, "assets.js.bootstrap.min.js"),
            "assets/js/popper.min.js" => Resx.GetStream(Id, "assets.js.popper.min.js"),
            _ => throw new FileNotFoundException()
        };
    }
}