using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Backend.Configuration;
using Fuyu.Common.Collections;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fuyu.Backend.EFTMain.Repositories;

public class JsonAccountRepository : IAccountRepository
{
    private readonly ThreadList<EftAccount> _accounts;
    private readonly EftConfiguration _config;
    private readonly ISessionRepository _sessions;

    public JsonAccountRepository(
        ISessionRepository sessions,
        ILogger<JsonAccountRepository> logger,
        IOptions<EftConfiguration> config)
    {
        _sessions = sessions;
        _config = config.Value;
        _accounts = new ThreadList<EftAccount>();

        if (!VFS.DirectoryExists(_config.AccountsPath))
        {
            VFS.CreateDirectory(_config.AccountsPath);
        }

        var files = VFS.GetFiles(_config.AccountsPath);
        foreach (var filepath in files)
        {
            var json = VFS.ReadTextFile(filepath);
            var account = Json.Parse<EftAccount>(json);
            _accounts.Add(account);
        }
    }

    public Task<List<EftAccount>> GetAllAsync()
    {
        return Task.FromResult(_accounts.ToList());
    }

    public async Task<EftAccount> GetByIdAsync(int accountId)
    {
        var accounts = await GetAllAsync();
        return accounts.FirstOrDefault(a => a.Id == accountId);
    }

    public async Task<EftAccount> GetBySessionAsync(string sessionId)
    {
        var aid = await _sessions.GetAccountIdAsync(sessionId);
        var accounts = await GetAllAsync();
        return accounts.FirstOrDefault(i => i.Id == aid);
    }

    public async Task AddOrUpdateAsync(EftAccount account)
    {
        var accounts = await GetAllAsync();
        var existingIndex = -1;

        for (var i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].Id == account.Id)
            {
                existingIndex = i;
                break;
            }
        }

        if (existingIndex >= 0)
        {
            _accounts.TrySet(existingIndex, account);
        }
        else
        {
            _accounts.Add(account);
        }

        await SaveAsync(account);
    }

    public async Task RemoveAsync(EftAccount account)
    {
        var accounts = await GetAllAsync();
        for (var i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].Id == account.Id)
            {
                _accounts.TryRemoveAt(i);
                break;
            }
        }

        // Remove file
        var filepath = $"{_config.AccountsPath}{account.Id}.json";
        if (VFS.Exists(filepath))
        {
            VFS.DeleteFile(filepath);
        }
    }

    public Task SaveAsync(EftAccount account)
    {
        var json = Json.Stringify(account);
        var filepath = $"{_config.AccountsPath}{account.Id}.json";
        VFS.WriteTextFile(filepath, json);

        return Task.CompletedTask;
    }

    public async Task<int> GetNewAccountIdAsync()
    {
        var accounts = await GetAllAsync();

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
}