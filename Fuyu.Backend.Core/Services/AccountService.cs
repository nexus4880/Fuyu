using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.Core.Models.Accounts;
using Fuyu.Backend.Core.Models.Responses;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Backend.Models.Requests;
using Fuyu.Common.Backend.Models.Responses;
using Fuyu.Common.Hashing;
using Fuyu.Common.Networking;
using Fuyu.Common.Services;

namespace Fuyu.Backend.Core.Services;

public class AccountService
{
    // TODO:
    // * account login state tracking
    // -- seionmoya, 2024/09/02

    private readonly ICoreAccountRepository _accounts;
    private readonly ICoreSessionRepository _sessions;
    private readonly RequestService _requestService;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public AccountService(ICoreAccountRepository accounts, ICoreSessionRepository sessions)
    {
        _accounts = accounts;
        _sessions = sessions;
        _requestService = RequestService.Instance;
        var eftHttpClient = new HttpClient("https://localhost:44301");
        _requestService.AddOrSetClient("eft", eftHttpClient);
    }

    public async Task<int> AccountExistsAsync(string username)
    {
        var lowerUsername = username.ToLowerInvariant();
        var accounts = await _accounts.GetAllAsync();

        // find account
        var found = new List<Account>();

        foreach (var account in accounts)
        {
            if (account.Username == lowerUsername)
            {
                found.Add(account);
            }
        }

        if (found.Count == 0)
        {
            // no account
            return -1;
        }
        else
        {
            // account exists
            return found[0].Id;
        }
    }

    public async Task<AccountLoginResponse> LoginAccountAsync(string username, string password)
    {
        // find account
        var accountId = await AccountExistsAsync(username);

        if (accountId == -1)
        {
            // account doesn't exist
            return new AccountLoginResponse()
            {
                Status = ELoginStatus.AccountNotFound,
                SessionId = string.Empty
            };
        }

        // validate password
        var account = await _accounts.GetByIdAsync(accountId);

        if (account.Password != password)
        {
            // password is wrong
            return new AccountLoginResponse()
            {
                Status = ELoginStatus.AccountNotFound,
                SessionId = string.Empty
            };
        }

        // validate status
        if (account.IsBanned)
        {
            // account is banned
            return new AccountLoginResponse()
            {
                Status = ELoginStatus.AccountBanned,
                SessionId = string.Empty
            };
        }

        // find active account session
        var sessions = await _sessions.GetAllAsync();

        foreach (var kvp in sessions)
        {
            if (kvp.Value == accountId)
            {
                return new AccountLoginResponse()
                {
                    Status = ELoginStatus.SessionAlreadyExists,
                    SessionId = kvp.Key
                };
            }
        }

        // create new account session
        // NOTE: Instead fully mimicking EFT's id (hwid+timestamp hash), I
        //       decided to generate a new MongoId for each login.
        // -- seionmoya, 2024/09/02
        var sessionId = new MongoId(accountId).ToString();

        await _sessions.SetAsync(sessionId, accountId);

        return new AccountLoginResponse()
        {
            Status = ELoginStatus.Success,
            SessionId = sessionId.ToString()
        };
    }

    private async Task<int> GetNewAccountIdAsync()
    {
        var accounts = await _accounts.GetAllAsync();

        // using linq because sorting otherwise takes up too much code
        var sorted = accounts.OrderBy(account => account.Id).ToArray();

        // find all gap entries
        var found = new List<int>();

        for (var i = 0; i < sorted.Length; ++i)
        {
            if (sorted[i].Id != i)
            {
                found.Add(sorted[i].Id);
            }
        }

        if (found.Count > 0)
        {
            // use first gap entry
            return found[0];
        }
        else
        {
            // use new entry
            return sorted.Length;
        }
    }

    public async Task<ERegisterStatus> RegisterAccountAsync(string username, string password)
    {
        // validate username
        if (await AccountExistsAsync(username) != -1)
        {
            return ERegisterStatus.AlreadyExists;
        }

        var usernameStatus = AccountValidationService.ValidateUsername(username);

        if (usernameStatus != ERegisterStatus.Success)
        {
            return usernameStatus;
        }

        // validate password
        var passwordStatus = AccountValidationService.ValidatePassword(password);

        if (passwordStatus != ERegisterStatus.Success)
        {
            return passwordStatus;
        }

        var hashedPassword = Sha256.Generate(password);
        var account = new Account()
        {
            Id = await GetNewAccountIdAsync(),
            Username = username.ToLowerInvariant(),
            Password = hashedPassword,
            Games = [],
            IsBanned = false
        };

        await _accounts.AddOrUpdateAsync(account);

        return ERegisterStatus.Success;
    }

    public async Task<AccountGameRegisterResponse> RegisterGameAsync(string sessionId, string game, string edition)
    {
        var account = await _accounts.GetBySessionAsync(sessionId);

        // register game
        var request = new FuyuGameRegisterRequest()
        {
            Username = account.Username,
            Edition = edition
        };
        var response = _requestService.Post<FuyuGameRegisterResponse>(game, "/fuyu/game/register", request);
        var accountId = response.AccountId;

        // set or add accountId
        if (account.Games.ContainsKey(game))
        {
            account.Games[game] = accountId;
        }
        else
        {
            account.Games.Add(game, accountId);
        }

        // store result
        await _accounts.AddOrUpdateAsync(account);

        return new AccountGameRegisterResponse()
        {
            AccountId = accountId
        };
    }

    public async Task<AccountGetResponse> GetStrippedAccountAsync(string sessionId)
    {
        var account = await _accounts.GetBySessionAsync(sessionId);

        var strippedAccount = new AccountGetResponse()
        {
            Username = account.Username,
            Games = account.Games
        };

        return strippedAccount;
    }
}