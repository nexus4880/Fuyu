using Fuyu.Common.Launcher.Models.Messages;
using Fuyu.Common.Launcher.Models.Pages;
using Fuyu.Common.Serialization;

namespace Fuyu.Launcher.Core.Pages;

public class IndexPage : AbstractPage
{
    protected override string Id { get; } = "Fuyu.Launcher";
    protected override string Path { get; } = "index.html";

    protected override void HandleMessage(string message)
    {
        var data = Json.Parse<Message>(message);

        switch (data.Type)
        {
            case "LOADED_PAGE":
                OnLoadedPageMessage(message);
                return;
        }
    }

    void OnLoadedPageMessage(string message)
    {
        // var body = Json.Parse<LoadedPageMessage>(message);
        var page = "account-login.html";
        NavigationService.NavigateInternal(page);
    }
}