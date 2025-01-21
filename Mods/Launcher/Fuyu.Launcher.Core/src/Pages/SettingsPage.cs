using System.IO;
using System.Text;
using Fuyu.Common.IO;
using Fuyu.Common.Launcher.Models.Messages;
using Fuyu.Common.Launcher.Models.Pages;
using Fuyu.Common.Launcher.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Launcher.Core.Pages;

public class SettingsPage : AbstractPage
{
    protected override string Id { get; } = "Fuyu.Launcher.Core";
    protected override string Path { get; } = "settings.html";

    protected override Stream LoadContent(string path)
    {
        var template = Resx.GetText(Id, Path);
        var html = SettingsService.Instance.GeneratePage(template);
        var bytes = Encoding.UTF8.GetBytes(html);
        return new MemoryStream(bytes);
    }

    protected override void HandleMessage(string message)
    {
        var data = Json.Parse<Message>(message);

        switch (data.Type)
        {
            case "SAVE_SETTINGS":
                OnSaveSettingsMessage(message);
                return;
        }
    }

    void OnSaveSettingsMessage(string message)
    {
        var body = Json.Parse<SaveSettingsMessage>(message);

        SettingsService.Instance.SaveSettings(body);
        NavigationService.NavigatePrevious();
    }
}