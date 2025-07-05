using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Repositories.Abstractions;

public interface IItemTemplateRepository : IRepository
{
    Task<Dictionary<MongoId, ItemTemplate>> GetAllAsync();
    Task<ItemTemplate> GetItemTemplateAsync(MongoId id);
}