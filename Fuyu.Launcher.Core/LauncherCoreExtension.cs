using System.IO;
using System.Threading.Tasks;
using Fuyu.Common.IO;
using Fuyu.Common.Launcher.Services;
using Fuyu.Common.Networking;
using Fuyu.Common.Services;
using Fuyu.DependencyInjection.Attributes;
using Fuyu.Launcher.Core.Pages;

namespace Fuyu.Launcher.Core;

public class LauncherCoreExtension
{
    private static readonly string _resourceId = "Fuyu.Launcher.Core";

    private readonly ContentService _contentService;
    private readonly RequestService _requestService;

    [Injectable]
    public LauncherCoreExtension(
        [Inject] ContentService contentService,
        [Inject] RequestService requestService
        )
    {
        _contentService = contentService;
        _requestService = requestService;
    }

    public Task Initialize()
    {
        Resx.SetSource(_resourceId, this.GetType().Assembly);

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
            "assets/css/bootstrap.min.css" => Resx.GetStream(_resourceId, "assets.css.bootstrap.min.css"),
            "assets/css/styles.css" => Resx.GetStream(_resourceId, "assets.css.styles.css"),
            "assets/js/bootstrap.min.js" => Resx.GetStream(_resourceId, "assets.js.bootstrap.min.js"),
            "assets/js/popper.min.js" => Resx.GetStream(_resourceId, "assets.js.popper.min.js"),
            _ => throw new FileNotFoundException()
        };
    }
}