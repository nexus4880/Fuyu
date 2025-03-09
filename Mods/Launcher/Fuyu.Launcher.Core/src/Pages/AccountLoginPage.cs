using System;
using Fuyu.Backend.Core.Models.Responses;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Launcher.Models.Messages;
using Fuyu.Common.Launcher.Models.Pages;
using Fuyu.Common.Launcher.Services;
using Fuyu.Common.Serialization;
using Fuyu.Launcher.Core.Models.Accounts;
using Fuyu.Launcher.Core.Models.Messages;
using Fuyu.Launcher.Core.Models.Replies;
using Fuyu.Launcher.Core.Models.Requests;
using Fuyu.Launcher.Core.Networking;

namespace Fuyu.Launcher.Core.Pages;

public class AccountLoginPage : AbstractPage
{
    protected override string Id { get; } = "Fuyu.Launcher.Core";
    protected override string Path { get; } = "account-login.html";

    protected override void HandleMessage(string message)
    {
        var data = Json.Parse<Message>(message);

        Action<string> callback = data.Type switch
        {
            "LOGIN_CORE" => OnLoginAccountMessage,
            _ => throw new Exception($"Unhandled message {message}")
        };

        if (callback != null)
        {
            callback(message);
        }
    }

    void OnLoginAccountMessage(string message)
    {
        var body = Json.Parse<LoginAccountMessage>(message);

        // validate input for null/empty
        if (string.IsNullOrWhiteSpace(body.Username))
        {
            SendLoginErrorReply("Empty username");
            return;
        }

        if (string.IsNullOrWhiteSpace(body.Password))
        {
            SendLoginErrorReply("Empty password");
            return;
        }

        // get request
        var hashedPassword = Sha256.Generate(body.Password);
        var request = new AccountLoginRequest()
        {
            Username = body.Username,
            Password = hashedPassword
        };

        // receive response
        AccountLoginResponse response;
        try
        {
            response = RequestService.Post<AccountLoginResponse>("core", "/account/login", request);
        }
        catch (Exception ex)
        {
            SendLoginErrorReply("There is a connection issue.");
            Terminal.WriteLine(ex);
            return;
        }

        // handle response
        if (response.Status == ELoginStatus.Success)
        {
            var coreClient = new CoreHttpClient("https://localhost:44300", response.SessionId);
            RequestService.AddOrSetClient("core", coreClient);

            var page = "account-games.html";
            NavigationService.NavigateInternal(page);
        }
        else
        {
            var errorMessage = response.Status switch
            {
                ELoginStatus.AccountBanned => "Account is banned.",
                ELoginStatus.AccountNotFound => "Account is not found.",
                ELoginStatus.SessionAlreadyExists => "Account is already logged in.",
                _ => throw new Exception("Unhandled case")
            };

            SendLoginErrorReply(errorMessage);
        }
    }

    void SendLoginErrorReply(string errorMessage)
    {
        var reply = new LoginAccountReply
        {
            Type = "LOGIN_ERROR",
            Message = errorMessage
        };

        var json = Json.Stringify(reply);
        MessageService.SendMessage(json);
    }
}