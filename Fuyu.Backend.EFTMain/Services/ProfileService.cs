using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Services;

public class ProfileService
{
    private readonly IGameDataRepository _gameData;
    private readonly IProfileRepository _profiles;
    private readonly IAccountRepository _accounts;
    private readonly IItemTemplateRepository _itemTemplates;
    private readonly ItemService _itemService;
    private readonly ItemFactoryService _itemFactoryService;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    public ProfileService(
        IGameDataRepository gameData,
        IProfileRepository profiles,
        IAccountRepository accounts,
        IItemTemplateRepository itemTemplates,
        ItemService itemService,
        ItemFactoryService itemFactoryService
        )
    {
        _gameData = gameData;
        _profiles = profiles;
        _accounts = accounts;
        _itemTemplates = itemTemplates;
        _itemService = itemService;
        _itemFactoryService = itemFactoryService;
    }

    public async Task<string> WipeProfile(string sessionId, string side, string headId, string voiceId)
    {
        var account = await _accounts.GetBySessionAsync(sessionId);
        var profile = await _profiles.GetActiveProfileAsync(sessionId);
        var pmcId = profile.Pmc._id;
        var savageId = profile.Savage._id;

        // create profiles
        var wipeProfiles = await _gameData.GetWipeProfilesAsync(account.Edition);

        profile.Savage = wipeProfiles[EPlayerSide.Savage].Clone();

        // NOTE: Case-sensitive
        // -- seionmoya, 2024-10-13
        switch (side)
        {
            case "Bear":
                profile.Pmc = wipeProfiles[EPlayerSide.Bear].Clone();
                break;

            case "Usec":
                profile.Pmc = wipeProfiles[EPlayerSide.Usec].Clone();
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

        await InitializeInventoryMatricesAsync(profile.Pmc);

        // wipe done
        profile.ShouldWipe = false;

        // store profile
        await _profiles.AddOrUpdateAsync(profile);

        return profile.Pmc._id;
    }

    public async Task InitializeInventoryMatricesAsync(Profile profile)
    {
        if (profile.Inventory is null)
        {
            return;
        }

        foreach (var item in profile.Inventory.Items)
        {
            var itemTemplate = await _itemTemplates.GetItemTemplateAsync(item.TemplateId);
            var props = _itemFactoryService.GetItemProperties<CompoundItemItemProperties>(itemTemplate);
            if (props.Grids.Count > 0)
            {
                var items = _itemService.GetItemAndChildren(profile.Inventory.Items, item);
                item.InitializeMatrices(props.Grids, items);
            }
        }
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