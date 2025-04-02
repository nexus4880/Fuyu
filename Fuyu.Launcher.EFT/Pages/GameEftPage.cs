using System.Diagnostics;
using Fuyu.Common.Launcher.Models.Messages;
using Fuyu.Common.Launcher.Models.Pages;
using Fuyu.Common.Serialization;
using Fuyu.Launcher.EFT.Models.Configs;
using Fuyu.Launcher.EFT.Models.Messages;
using Fuyu.Launcher.EFT.Models.Replies;
using Fuyu.Launcher.EFT.Models.Requests;
using Fuyu.Launcher.EFT.Models.Responses;

namespace Fuyu.Launcher.EFT.Pages;

public class GameEftPage : AbstractPage
{
    protected override string Id { get; } = "Fuyu.Launcher.EFT";
    protected override string Path { get; } = "game-eft.html";

    protected override void HandleMessage(string message)
    {
        var data = Json.Parse<Message>(message);

        switch (data.Type)
        {
            case "LAUNCH_GAME":
                OnGameLaunchMessage(message);
                return;
        }
    }

    void OnGameLaunchMessage(string message)
    {
        var body = Json.Parse<LaunchGameMessage>(message);

        // request sessionId
        var accountId = RequestGameAccountId("eft", "unheard");
        var gameSessionId = RequestSessionId(accountId);

        // TODO: Keep track of game lifecycle
        // -- seionmoya, 2025-01-11
        var gamepath = ModConfig.Instance.GamePath;
        var address = ModConfig.Instance.Address;
        var process = GetEftProcess(gamepath, gameSessionId, address);
        process.Start();

        ReplyLaunchSuccess();
    }

    void ReplyLaunchSuccess()
    {
        var reply = new LaunchGameReply
        {
            Type = "LAUNCH_SUCCESS",
            Message = string.Empty
        };

        var json = Json.Stringify(reply);
        MessageService.SendMessage(json);
    }

    int RequestGameAccountId(string game, string edition)
    {
        var account = RequestService.Get<AccountGetResponse>("core", "/account/get");

        if (account.Games.TryGetValue(game, out int? gameAccountId))
        {
            // find existing game account
            return gameAccountId.Value;
        }
        else
        {
            // register game
            var request = new AccountGameRegisterRequest()
            {
                Game = game,
                Edition = edition
            };
            var response = RequestService.Post<AccountGameRegisterResponse>("core", "/account/game/register", request);

            return response.AccountId;
        }
    }

    string RequestSessionId(int accountId)
    {
        var request = new FuyuGameLoginRequest()
        {
            AccountId = accountId
        };
        var response = RequestService.Post<FuyuGameLoginResponse>("eft", "/fuyu/game/login", request);

        var sessionId = response.SessionId;
        return sessionId;
    }

    Process GetEftProcess(string cwd, string sessionId, string address)
    {
        // set filepath
        var processStartInfo = new ProcessStartInfo()
        {
            FileName = $"{cwd}/EscapeFromTarkov.exe",
            WorkingDirectory = cwd
        };

        // add token
        processStartInfo.ArgumentList.Add($"-token={sessionId}");

        // add eft startup config
        var config = new EFTStartupConfig()
        {
            BackendUrl = address,
            Version = "live",
            MatchingVersion = "live"
        };
        var json = Json.Stringify(config);

        processStartInfo.ArgumentList.Add($"-config={json}");

        // create process
        return new Process()
        {
            StartInfo = processStartInfo
        };
    }
}