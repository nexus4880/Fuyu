using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Factories.Abstractions;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Common.Hashing;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Backend.BSG.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Factories;

public class ProfileFactory : IProfileFactory
{
    private readonly IGameDataRepository _gameData;
    private readonly IProfileRepository _profiles;

    public ProfileFactory(
        IGameDataRepository gameData,
        IProfileRepository profiles
        )
    {
        _gameData = gameData;
        _profiles = profiles;
    }

    public async Task<EftProfile> CreateProfileAsync(int accountId)
    {
        var builds = await _gameData.GetDefaultBuildsAsync();
        var profile = new EftProfile()
        {
            Pmc = new Profile(),
            Savage = new Profile(),
            Customization = [],
            Builds = builds,
            ShouldWipe = true
        };

        // generate new ids
        var mongoId = MongoId.Generate();
        var pmcId = mongoId.ToString();
        mongoId = mongoId.Next();
        var savageId = mongoId.ToString();

        // set profile info
        profile.Pmc._id = pmcId;
        profile.Pmc.aid = accountId;

        profile.Savage._id = savageId;
        profile.Savage.aid = accountId;

        await _profiles.AddOrUpdateAsync(profile);

        return profile;
    }
}
