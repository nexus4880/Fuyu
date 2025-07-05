using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Services;

public class AccountService
{
    // TODO:
    // * account login state tracking
    // -- seionmoya, 2024/09/06

    private readonly IAccountRepository _accounts;
    private readonly ISessionRepository _sessions;
    private readonly ProfileService _profileService;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public AccountService(IAccountRepository accounts, ISessionRepository sessions, ProfileService profileService)
    {
        _sessions = sessions;
        _accounts = accounts;
        _profileService = profileService;
    }

    public async Task<string> LoginAccount(int accountId)
    {
        if (accountId == -1)
        {
            // account doesn't exist
            return string.Empty;
        }

        // find active account session
        var sessions = await _sessions.GetAllAsync();

        foreach (var kvp in sessions)
        {
            if (kvp.Value == accountId)
            {
                // session already exists
                return kvp.Key;
            }
        }

        // create new account session
        // NOTE: MongoId's are used internally, but EFT's launcher uses
        //       a different ID system (hwid+timestamp hash). Instead of
        //       fully mimicking this, I decided to generate a new MongoId
        //       for each login.
        // -- seionmoya, 2024/09/02
        var sessionId = new MongoId(accountId).ToString();
        await _sessions.SetAsync(sessionId, accountId);

        return sessionId.ToString();
    }

    public async Task<int> RegisterAccountAsync(string username, string edition)
    {
        var accountId = await _accounts.GetNewAccountIdAsync();

        // create profiles
        var pvpId = await _profileService.CreateProfile(accountId);
        var pveId = await _profileService.CreateProfile(accountId);

        // create account   
        var account = new EftAccount()
        {
            Id = accountId,
            Edition = edition,
            Username = username,
            PvpId = pvpId,
            PveId = pveId,
            CurrentSession = ESessionMode.Pve
        };

        await _accounts.AddOrUpdateAsync(account);

        return accountId;
    }
}