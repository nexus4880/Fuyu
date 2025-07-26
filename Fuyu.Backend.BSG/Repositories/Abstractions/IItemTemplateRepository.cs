using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Repositories.Abstractions;

public interface IItemTemplateRepository
{
    Task<Dictionary<MongoId, ItemTemplate>> GetAllAsync();
    Task<ItemTemplate> GetItemTemplateAsync(MongoId id);
}