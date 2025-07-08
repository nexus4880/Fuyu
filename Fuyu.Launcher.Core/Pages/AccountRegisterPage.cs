using System;
using System.Diagnostics;
using Fuyu.Common.Launcher.Models.Messages;
using Fuyu.Common.Launcher.Models.Pages;
using Fuyu.Common.Launcher.Services;
using Fuyu.Common.Serialization;
using Fuyu.Launcher.Core.Models.Accounts;
using Fuyu.Launcher.Core.Models.Messages;
using Fuyu.Launcher.Core.Models.Replies;
using Fuyu.Launcher.Core.Models.Requests;
using Fuyu.Launcher.Core.Models.Response;

namespace Fuyu.Launcher.Core.Pages;

public class AccountRegisterPage : AbstractPage
{
    protected override string Id { get; } = "Fuyu.Launcher.Core";
    protected override string Path { get; } = "account-register.html";

    protected override void HandleMessage(string message)
    {
        var data = Json.Parse<Message>(message);

        switch (data.Type)
        {
            case "REGISTER_CORE":
                OnAccountRegisterMessage(message);
                return;
        }
    }

    void OnAccountRegisterMessage(string message)
    {
        var data = Json.Parse<RegisterAccountMessage>(message);

        // validate input for null/empty
        if (string.IsNullOrWhiteSpace(data.Username))
        {
            SendRegisterErrorReply("Username is empty");
            return;
        }

        if (string.IsNullOrWhiteSpace(data.Password))
        {
            SendRegisterErrorReply("Password is empty");
            return;
        }

        // get request
        var request = new AccountRegisterRequest()
        {
            Username = data.Username,
            Password = data.Password
        };

        // receive response
        AccountRegisterResponse response;
        try
        {
            response = RequestService.Post<AccountRegisterResponse>("core", "/account/register", request);
        }
        catch (Exception ex)
        {
            SendRegisterErrorReply("There is a connection issue.");
            Debug.WriteLine(ex);
            return;
        }

        if (response.Status == ERegisterStatus.Success)
        {
            SendRegisterSuccessReply();
        }
        else
        {
            var errorMessage = response.Status switch
            {
                ERegisterStatus.UsernameEmpty => "No username provided.",
                ERegisterStatus.UsernameTooShort => "Username is too short.",
                ERegisterStatus.UsernameTooLong => "Username is too long.",
                ERegisterStatus.UsernameInvalidCharacter => "Username contains invalid characters.",
                ERegisterStatus.PasswordEmpty => "No password provided.",
                ERegisterStatus.PasswordTooShort => "Password is too short.",
                ERegisterStatus.PasswordTooLong => "Password is too long.",
                ERegisterStatus.PasswordMissingLowerCase => "Password doesn't contain lower-case characters.",
                ERegisterStatus.PasswordMissingUpperCase => "Password doesn't contain upper-case characters.",
                ERegisterStatus.PasswordMissingDigit => "Password doesn't contain digits.",
                ERegisterStatus.PasswordMissingSpecial => "Password doesn't contain special characters.",
                ERegisterStatus.AlreadyExists => "Account already exists.",
                _ => throw new Exception($"{response.Status} is not being handled")
            };

            SendRegisterErrorReply(errorMessage);
        }
    }

    void SendRegisterErrorReply(string errorMessage)
    {
        var reply = new RegisterAccountReply
        {
            Type = "REGISTER_ERROR",
            Message = errorMessage
        };

        var json = Json.Stringify(reply);
        MessageService.SendMessage(json);
    }

    void SendRegisterSuccessReply()
    {
        var reply = new RegisterAccountReply
        {
            Type = "REGISTER_SUCCESS",
            Message = "Success! You can now login."
        };

        var json = Json.Stringify(reply);
        MessageService.SendMessage(json);
    }
}