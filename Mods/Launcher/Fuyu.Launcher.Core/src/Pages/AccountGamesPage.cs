using System;
using Fuyu.Common.Launcher.Models.Messages;
using Fuyu.Common.Launcher.Models.Pages;
using Fuyu.Common.Serialization;
using Fuyu.Launcher.Core.Models.Messages;

namespace Fuyu.Launcher.Core.Pages;

public class AccountLibraryPage : AbstractPage
{
    protected override string Id { get; } = "Fuyu.Launcher.Core";
    protected override string Path { get; } = "account-games.html";

    protected override void HandleMessage(string message)
    {
        var data = Json.Parse<Message>(message);

        switch (data.Type)
        {
            case "NAVIGATE_GAME":
                OnViewAccountGameMessage(message);
                return;
        }
    }

    void OnViewAccountGameMessage(string message)
    {
        var body = Json.Parse<ViewAccountGameMessage>(message);

        // TODO: GameService navigation system
        // -- seionmoya, 2025-01-10
        switch (body.Game)
        {
            case "eft":
                var page = "game-eft.html";
                NavigationService.NavigateInternal(page);
                return;

            default:
                throw new Exception($"{body.Game} is not supported");
        }
    }
}