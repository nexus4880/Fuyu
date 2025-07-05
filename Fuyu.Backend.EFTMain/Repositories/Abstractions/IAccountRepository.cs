using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;

namespace Fuyu.Backend.EFTMain.Repositories.Abstractions;

public interface IAccountRepository : IRepository
{
    Task<List<EftAccount>> GetAllAsync();
    Task<EftAccount> GetByIdAsync(int accountId);
    Task<EftAccount> GetBySessionAsync(string sessionId);
    Task AddOrUpdateAsync(EftAccount account);
    Task RemoveAsync(EftAccount account);
    Task SaveAsync(EftAccount account);
    Task<int> GetNewAccountIdAsync();
}