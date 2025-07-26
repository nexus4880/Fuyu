using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Collections;

namespace Fuyu.Backend.Core.Repositories;

public class JsonCoreSessionRepository : ICoreSessionRepository
{
    private readonly ThreadDictionary<string, int> _sessions;

    public JsonCoreSessionRepository()
    {
        _sessions = new ThreadDictionary<string, int>();
    }

    public Task<bool> ExistsAsync(string sessionId)
    {
        return Task.FromResult(_sessions.ContainsKey(sessionId));
    }

    public Task<int> GetAccountIdAsync(string sessionId)
    {
        if (_sessions.TryGet(sessionId, out var accountId))
        {
            return Task.FromResult(accountId);
        }

        throw new Exception($"Failed to find aid from sessionId {sessionId}");
    }

    public Task<Dictionary<string, int>> GetAllAsync()
    {
        return Task.FromResult(_sessions.ToDictionary());
    }

    public Task RemoveAsync(string sessionId)
    {
        _sessions.Remove(sessionId);
        return Task.CompletedTask;
    }

    public Task SetAsync(string sessionId, int accountId)
    {
        _sessions.Set(sessionId, accountId);
        return Task.CompletedTask;
    }
}