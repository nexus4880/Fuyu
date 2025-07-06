using System.Diagnostics;
using System.IO;
using System.Windows;
using Fuyu.Common.IO;
using Fuyu.Common.Launcher.Services;
using Fuyu.Common.Services;
using Fuyu.Launcher.Core;
using Fuyu.Launcher.EFT;

namespace Fuyu.Launcher;

public partial class MainWindow : Window
{
    private readonly ContentService _contentService;
    private readonly MessageService _messageService;
    private readonly NavigationService _navigationService;
    private readonly WebViewService _webViewService;
    private readonly RequestService _requestService;
    private readonly SettingsService _settingsService;

    public MainWindow(
        ContentService contentService,
        MessageService messageService,
        NavigationService navigationService,
        WebViewService webViewService,
        RequestService requestService,
        SettingsService settingsService
        )
    {
        _contentService = contentService;
        _messageService = messageService;
        _navigationService = navigationService;
        _webViewService = webViewService;
        _requestService = requestService;
        _settingsService = settingsService;

        // initialize page
        InitializeComponent();
        InitializeAsync();
    }

    // lazy initialize _webview
    async void InitializeAsync()
    {
        //Terminal.SetLogConfig("Fuyu/Logs/Launcher.log");

        // initialize webview
        await browser.EnsureCoreWebView2Async(null);
        var webview = browser.CoreWebView2;

        // initialize services
        _webViewService.Initialize(webview);
        _navigationService.Initialize(webview);
        _messageService.Initialize(webview);

        // set content
        Resx.SetSource("Fuyu.Launcher", this.GetType().Assembly);
        _contentService.SetOrAddLoader("index.html", LoadContent);
        _contentService.SetOrAddLoader("favicon.ico", LoadContent);

        // load mods
        Debug.WriteLine("Loading mods...");

#if DEBUG
        // NOTE: assumes running inside VSCode or VS2022+
        var modPath = "../../../../../Mods/Launcher";
#else
        var modPath = "./Fuyu/Mods/Launcher";
#endif

        var core = new LauncherCoreExtension(_contentService, _requestService, _settingsService);
        await core.Initialize();

        var launcher = new EFTLauncherExtension(_contentService, _requestService, _settingsService);
        await launcher.Initialize();

        // load initial page
        var url = _navigationService.GetInternalUrl("index.html");
        _navigationService.NavigateInternal(url);
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