using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Repositories.Abstractions;

public interface ITraderRepository
{
    Task<Dictionary<MongoId, TraderTemplate>> GetTraderTemplatesAsync();
    Task<Dictionary<MongoId, TraderAssort>> GetTraderAssortsAsync();

    public Task<TraderTemplate> GetTraderTemplateAsync(MongoId id);
    public Task<TraderAssort> GetTraderAssortAsync(MongoId id);
}