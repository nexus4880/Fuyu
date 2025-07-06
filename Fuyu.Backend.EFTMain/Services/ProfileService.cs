using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Services;

public class ProfileService
{
    private readonly IGameDataRepository _gameData;
    private readonly IProfileRepository _profiles;
    private readonly IAccountRepository _accounts;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public ProfileService(IGameDataRepository gameData, IProfileRepository profiles, IAccountRepository accounts)
    {
        _gameData = gameData;
        _profiles = profiles;
        _accounts = accounts;
    }

    public async Task<string> CreateProfile(int accountId)
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

        // store profile
        await _profiles.AddOrUpdateAsync(profile);

        return pmcId;
    }

    public async Task<string> WipeProfile(string sessionId, string side, string headId, string voiceId)
    {
        var account = await _accounts.GetBySessionAsync(sessionId);
        var profile = await _profiles.GetActiveProfileAsync(sessionId);
        var pmcId = profile.Pmc._id;
        var savageId = profile.Savage._id;

        // create profiles
        var edition = await _gameData.GetWipeProfilesAsync(account.Edition);

        profile.Savage = edition[EPlayerSide.Savage].Clone();

        // NOTE: Case-sensitive
        // -- seionmoya, 2024-10-13
        switch (side)
        {
            case "Bear":
                profile.Pmc = edition[EPlayerSide.Bear];
                break;

            case "Usec":
                profile.Pmc = edition[EPlayerSide.Usec];
                break;

            default:
                throw new Exception("Unsupported faction");
        }

        // setup savage
        profile.Savage._id = savageId;
        profile.Savage.aid = account.Id;

        // setup pmc
        var voiceTemplate = await _gameData.GetCustomizationAsync(voiceId);

        profile.Pmc._id = pmcId;
        profile.Pmc.savage = savageId;
        profile.Pmc.aid = account.Id;
        profile.Pmc.Info.Nickname = account.Username;
        profile.Pmc.Info.LowerNickname = account.Username.ToLowerInvariant();
        profile.Pmc.Info.Voice = voiceTemplate._name;
        profile.Pmc.Customization.Head = headId;

        // wipe done
        profile.ShouldWipe = false;

        // store profile
        await _profiles.AddOrUpdateAsync(profile);

        return profile.Pmc._id;
    }

    /// <summary>
    /// Checks whether a nickname is valid for the client
    /// </summary>
    /// <param name="nickname">The new nickname</param>
    /// <param name="status">The status returned to the client</param>
    /// <returns>True if the nickname is valid</returns>
    public ENicknameChangeResult IsValidNickname(string nickname)
    {
        //TODO: Handle all results
        if (nickname.Length < 3)
        {
            return ENicknameChangeResult.TooShort;
        }

        if (nickname.Length > 15)
        {
            return ENicknameChangeResult.CharacterLimit;
        }

        return ENicknameChangeResult.Ok;
    }
}