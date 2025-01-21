using System;
using Microsoft.Web.WebView2.Core;
using Fuyu.Common.IO;

namespace Fuyu.Common.Launcher.Services;

public class NavigationService
{
    public static NavigationService Instance => _instance.Value;
    private static readonly Lazy<NavigationService> _instance = new(() => new NavigationService());

    public const string INTERNAL_DOMAIN = "http://launcher.fuyu.api";
    public string CurrentPage;
    public string PreviousPage;
    private CoreWebView2 _webview;

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private NavigationService()
    {
        CurrentPage = string.Empty;
        PreviousPage = string.Empty;
        _webview = null;
    }

    public void Initialize(CoreWebView2 webview)
    {
        _webview = webview;
    }

    public string GetHeaders(string path)
    {
        // get file extension from Uri
        var ext = VFS.GetFileExtension(path);

        // get header from file extension
        var headers = ext switch
        {
            ".html" => "Content-Type: text/html",
            ".css"  => "Content-Type: text/css",
            ".js"   => "Content-Type: application/javascript",
            ".ico"  => "Content-Type: image/x-icon",
            ".png"  => "Content-Type: image/png",
            _ => throw new NotSupportedException(ext)
        };

        return headers;
    }

    public bool IsInternalRequest(string url)
    {
        return url.StartsWith(INTERNAL_DOMAIN)
            || url.StartsWith("./")
            || url.StartsWith("../");
    }

    public string GetInternalPath(string url)
    {
        return url.Replace(INTERNAL_DOMAIN + "/", string.Empty);
    }

    public string GetInternalUrl(string path)
    {            
        return $"{INTERNAL_DOMAIN}/{path}";
    }

    void Navigate(string url)
    {
        #if DEBUG
        // show received message
        Terminal.WriteLine($"Backend redirect: {url}");
        #endif

        _webview.Navigate(url);
    }

    public void NavigatePrevious()
    {
        Navigate(PreviousPage);
    }

    public void NavigateInternal(string page)
    {
        var url = GetInternalUrl(page);
        Navigate(url);
    }
}