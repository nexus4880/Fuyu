using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.Core.Models.Accounts;

namespace Fuyu.Backend.EFTMain.Repositories.Abstractions;

public interface ICoreAccountRepository : IRepository
{
    Task<List<Account>> GetAllAsync();
    Task<Account> GetByIdAsync(int accountId);
    Task<Account> GetBySessionAsync(string sessionId);
    Task AddOrUpdateAsync(Account account);
    Task RemoveAsync(Account account);
}