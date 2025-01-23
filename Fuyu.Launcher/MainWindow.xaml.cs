using System.IO;
using System.Windows;
using Fuyu.Common.IO;
using Fuyu.Common.Launcher.Services;
using Fuyu.DependencyInjection;
using Fuyu.Modding;

namespace Fuyu.Launcher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        // initialize page
        InitializeComponent();
        InitializeAsync();
    }

    // lazy initialize _webview
    async void InitializeAsync()
    {
        // resolve dependencies
        var container = new DependencyContainer();

        Terminal.SetLogConfig("Fuyu.Launcher", "Fuyu/Logs/Launcher.log");

        var contentService = ContentService.Instance;
        var messageService = MessageService.Instance;
        var modManager = ModManager.Instance;
        var navigationService = NavigationService.Instance;
        var webViewService = WebViewService.Instance;

        // initialize webview
        await browser.EnsureCoreWebView2Async(null);
        var webview = browser.CoreWebView2;

        // initialize services
        webViewService.Initialize(webview);
        navigationService.Initialize(webview);
        messageService.Initialize(webview);

        // set content
        Resx.SetSource("Fuyu.Launcher", this.GetType().Assembly);
        contentService.SetOrAddLoader("index.html", LoadContent);
        contentService.SetOrAddLoader("favicon.ico", LoadContent);

        // load mods
        Terminal.WriteLine("Loading mods...");

#if DEBUG
        // NOTE: assumes running inside VSCode or VS2022+
        var modPath = "../../../../../Mods/Launcher";
#else
        var modPath = "./Fuyu/Mods/Launcher";
#endif

        modManager.AddMods(modPath);
        await modManager.Load(container);

        // load initial page
        var url = navigationService.GetInternalUrl("index.html");
        navigationService.NavigateInternal(url);
    }

    Stream LoadContent(string path)
    {
        return path switch
        {
            "index.html" => Resx.GetStream("Fuyu.Launcher", "index.html"),
            "favicon.ico" => Resx.GetStream("Fuyu.Launcher", "icon.ico"),
            _ => throw new FileNotFoundException()
        };
    }
}