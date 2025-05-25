using System;
using System.Collections.Generic;
using System.Linq;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Services;

public class AccountService
{
    // TODO:
    // * account login state tracking
    // -- seionmoya, 2024/09/06

    public static AccountService Instance => instance.Value;
    private static readonly Lazy<AccountService> instance = new(() => new AccountService());

    private readonly EftOrm _eftOrm;
    private readonly ProfileService _profileService;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private AccountService()
    {
        _eftOrm = EftOrm.Instance;
        _profileService = ProfileService.Instance;
    }

    public string LoginAccount(int accountId)
    {
        if (accountId == -1)
        {
            // account doesn't exist
            return string.Empty;
        }

        // find active account session
        var sessions = _eftOrm.GetSessions();

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
        _eftOrm.SetOrAddSession(sessionId, accountId);
        return sessionId.ToString();
    }

    private int GetNewAccountId()
    {
        var accounts = _eftOrm.GetAccounts();

        // using linq because sorting otherwise takes up too much code
        var sorted = accounts.OrderBy(account => account.Id).ToArray();

        // find all gap entries
        var found = new List<int>();

        // accounts must start at 1 as there are checks handle 0 as error in the client
        var offset = 1;

        for (var i = offset; i < sorted.Length; ++i)
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
            return sorted.Length + offset;
        }
    }

    public int RegisterAccount(string username, string edition)
    {
        var accountId = GetNewAccountId();

        // create profiles
        var pvpId = _profileService.CreateProfile(accountId);
        var pveId = _profileService.CreateProfile(accountId);

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

        _eftOrm.SetOrAddAccount(account);
        WriteToDisk(account);

        return accountId;
    }

    public void WriteToDisk(EftAccount account)
    {
        VFS.WriteTextFile(
            $"./Fuyu/Accounts/EFT/{account.Id}.json",
            Json.Stringify(account));
    }
}