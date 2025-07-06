using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Collections;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain.Repositories;

public class JsonEftSessionRepository : ISessionRepository
{
    private readonly ThreadDictionary<string, int> _sessions;

    public JsonEftSessionRepository(ILogger<JsonEftSessionRepository> logger)
    {
        _sessions = new ThreadDictionary<string, int>();
    }

    public Task<Dictionary<string, int>> GetAllAsync()
    {
        return Task.FromResult(_sessions.ToDictionary());
    }

    public Task<int> GetAccountIdAsync(string sessionId)
    {
        if (!_sessions.TryGet(sessionId, out var accountId))
        {
            throw new KeyNotFoundException($"Session not found: {sessionId}");
        }

        return Task.FromResult(accountId);
    }

    public Task SetAsync(string sessionId, int accountId)
    {
        _sessions.Set(sessionId, accountId);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string sessionId)
    {
        _sessions.Remove(sessionId);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string sessionId)
    {
        return Task.FromResult(_sessions.ContainsKey(sessionId));
    }
}