using System;
using Fuyu.Common.IO;
using Microsoft.Web.WebView2.Core;

namespace Fuyu.Common.Launcher.Services;

public class WebViewService
{
    public static WebViewService Instance => _instance.Value;
    private static readonly Lazy<WebViewService> _instance = new(() => new WebViewService());

    private readonly ContentService _contentService;
    private readonly MessageService _messageService;
    private readonly NavigationService _navigationService;

    private CoreWebView2 _webview;

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private WebViewService()
    {
        _contentService = ContentService.Instance;
        _messageService = MessageService.Instance;
        _navigationService = NavigationService.Instance;

        _webview = null;
    }

    public void Initialize(CoreWebView2 webview)
    {
        _webview = webview;

        // disable settings
        _webview.Settings.AreDefaultContextMenusEnabled = false;
        _webview.Settings.IsZoomControlEnabled = false;
        _webview.Settings.IsPinchZoomEnabled = false;
        _webview.Settings.IsSwipeNavigationEnabled = false;

        // add event listeners
        _webview.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        _webview.NavigationStarting += NavigationStarting;
        _webview.WebResourceRequested += WebResourceRequested;
        _webview.WebMessageReceived += WebMessageReceived;
    }

    // handle all events registered on NavigationStarting
    void NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
    {
        var url = args.Uri;

#if DEBUG
        Terminal.WriteLine($"Navigating to: {url}");
#endif

        if (!_navigationService.IsInternalRequest(url))
        {
            // block non-internal requests due to security concerns
            throw new Exception($"Blocked request {url}");
        }

        // update navigation
        _navigationService.PreviousPage = _navigationService.CurrentPage;
        _navigationService.CurrentPage = url;
    }

    // handle all events on registered AddWebResourceRequestedFilter
    void WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs args)
    {
        var url = args.Request.Uri;

#if DEBUG
        Terminal.WriteLine($"Requested resource: {url}");
#endif

        if (_navigationService.IsInternalRequest(url))
        {
            // grab the data
            var path = _navigationService.GetInternalPath(url);
            var headers = _navigationService.GetHeaders(path);
            var content = _contentService.Load(path);

            // send response
            args.Response = _webview.Environment.CreateWebResourceResponse(content, 200, "OK", headers);
            return;
        }
        else
        {
            // block non-internal requests due to security concerns
            throw new Exception($"Blocked request {url}");
        }
    }

    // handle message received from JS window.chrome._webview.postMessage(text)
    void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        var message = args.TryGetWebMessageAsString();
        var url = _navigationService.CurrentPage;

        if (_navigationService.IsInternalRequest(url))
        {
            var path = _navigationService.GetInternalPath(url);
            _messageService.HandleMessage(path, message);
            return;
        }
        else
        {
            // block non-internal messages due to security concerns
            throw new Exception($"Blocked message {url}");
        }
    }
}