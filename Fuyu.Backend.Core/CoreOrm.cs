using System;
using System.Collections.Generic;
using Fuyu.Backend.Core.Models.Accounts;

namespace Fuyu.Backend.Core;

public class CoreOrm
{
    public static CoreOrm Instance => instance.Value;
    private static readonly Lazy<CoreOrm> instance = new(() => new CoreOrm());

    private readonly CoreDatabase _coreDatabase;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private CoreOrm()
    {
        _coreDatabase = CoreDatabase.Instance;
    }

    #region Accounts
    public List<Account> GetAccounts()
    {
        return _coreDatabase.Accounts.ToList();
    }

    public Account GetAccount(string sessionId)
    {
        var accountId = GetSession(sessionId);
        if (!_coreDatabase.Accounts.TryGet(accountId, out var account))
        {
            throw new Exception($"Failed to get account with sessionID: {sessionId}");
        }

        return account;
    }

    public Account GetAccount(int accountId)
    {
        foreach (var entry in _coreDatabase.Accounts.ToList())
        {
            if (entry.Id == accountId)
            {
                return entry;
            }
        }

        throw new Exception($"Account with {accountId} does not exist.");
    }

    public void SetOrAddAccount(Account account)
    {
        var accounts = _coreDatabase.Accounts.ToList();

        for (var i = 0; i < accounts.Count; ++i)
        {
            if (accounts[i].Id == account.Id)
            {
                _coreDatabase.Accounts.TrySet(i, account);
                return;
            }
        }

        _coreDatabase.Accounts.Add(account);
    }

    public void RemoveAccount(int accountId)
    {
        var accounts = _coreDatabase.Accounts.ToList();

        for (var i = 0; i < accounts.Count; ++i)
        {
            if (accounts[i].Id == accountId)
            {
                _coreDatabase.Accounts.TryRemoveAt(i);
                return;
            }
        }
    }
    #endregion

    #region Sessions
    public Dictionary<string, int> GetSessions()
    {
        return _coreDatabase.Sessions.ToDictionary();
    }

    public int GetSession(string sessionId)
    {
        if (!_coreDatabase.Sessions.TryGet(sessionId, out var id))
        {
            // I don't want to have to restart the launcher every time I need a new session
            // -- nexus4880, 2025-1-26
#if DEBUG
            _coreDatabase.Sessions.Set(sessionId, id);
#else
            throw new Exception($"Failed to find ID for sessionId: {sessionId}");
#endif
        }

        return id;
    }

    public void SetOrAddSession(string sessionId, int accountId)
    {
        if (_coreDatabase.Sessions.ContainsKey(sessionId))
        {
            _coreDatabase.Sessions.Set(sessionId, accountId);
        }
        else
        {
            _coreDatabase.Sessions.Set(sessionId, accountId);
        }
    }

    public void RemoveSession(string sessionId)
    {
        _coreDatabase.Sessions.Remove(sessionId);
    }
    #endregion
}