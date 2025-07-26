using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;

namespace Fuyu.Backend.EFTMain.Factories.Abstractions;

public interface IProfileFactory
{
    Task<EftProfile> CreateProfileAsync(int accountId);
}