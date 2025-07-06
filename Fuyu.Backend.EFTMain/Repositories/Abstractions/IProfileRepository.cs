using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;

namespace Fuyu.Backend.EFTMain.Repositories.Abstractions;

public interface IProfileRepository : IRepository
{
    Task<List<EftProfile>> GetAllAsync();
    Task<EftProfile> GetByIdAsync(string profileId);
    Task<EftProfile> GetActiveProfileAsync(string sessionId);
    Task AddOrUpdateAsync(EftProfile profile);
    Task RemoveAsync(EftProfile profile);
}