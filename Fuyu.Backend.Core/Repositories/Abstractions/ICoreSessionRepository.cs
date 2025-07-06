using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fuyu.Backend.EFTMain.Repositories.Abstractions;

public interface ICoreSessionRepository : IRepository
{
    Task<Dictionary<string, int>> GetAllAsync();
    Task<int> GetAccountIdAsync(string sessionId);
    Task SetAsync(string sessionId, int accountId);
    Task RemoveAsync(string sessionId);
    Task<bool> ExistsAsync(string sessionId);
}