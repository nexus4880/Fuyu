using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.Core.Configuration;
using Fuyu.Backend.Core.Models.Accounts;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Collections;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Microsoft.Extensions.Options;

namespace Fuyu.Backend.Core.Repositories;

public class JsonCoreAccountRepository : ICoreAccountRepository
{
    private readonly ThreadDictionary<int, Account> _accounts;
    private readonly ICoreSessionRepository _sessions;

    public JsonCoreAccountRepository(
        IOptions<CoreConfiguration> config,
        ICoreSessionRepository sessions
        )
    {
        _accounts = new ThreadDictionary<int, Account>();
        _sessions = sessions;

        var accountFiles = VFS.GetFiles(config.Value.AccountsPath);
        foreach (var file in accountFiles)
        {
            var fileContents = VFS.ReadTextFile(file);
            var account = Json.Parse<Account>(fileContents);
            _accounts.Set(account.Id, account);
        }
    }

    public Task AddOrUpdateAsync(Account account)
    {
        _accounts.Set(account.Id, account);
        WriteToDisk(account);
        return Task.CompletedTask;
    }

    public Task<List<Account>> GetAllAsync()
    {
        var accounts = new List<Account>();
        foreach (var (_, account) in _accounts)
        {
            accounts.Add(account);
        }

        return Task.FromResult(accounts);
    }

    public Task<Account> GetByIdAsync(int accountId)
    {
        if (_accounts.TryGet(accountId, out var account))
        {
            return Task.FromResult(account);
        }

        throw new Exception($"Failed to find account with id {account}");
    }

    public async Task<Account> GetBySessionAsync(string sessionId)
    {
        var aid = await _sessions.GetAccountIdAsync(sessionId);
        if (_accounts.TryGet(aid, out var account))
        {
            return account;
        }

        throw new Exception($"Failed to find account with aid {aid}");
    }

    public Task RemoveAsync(Account account)
    {
        _accounts.Remove(account.Id);
        VFS.DeleteFile($"./Fuyu/Accounts/Core/{account.Id}.json");
        return Task.CompletedTask;
    }

    private void WriteToDisk(Account account)
    {
        VFS.WriteTextFile(
            $"./Fuyu/Accounts/Core/{account.Id}.json",
            Json.Stringify(account));
    }
}
