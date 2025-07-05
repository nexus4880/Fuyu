using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFT.Controllers.Http;

public class GameProfileNicknameChangeController : AbstractEftHttpController<GameProfileNicknameChangeRequest>
{
    private readonly ProfileService _profileService;
    private readonly IProfileRepository _profiles;

    public GameProfileNicknameChangeController(IProfileRepository profiles, ProfileService profileService) : base("/client/game/profile/nickname/change")
    {
        _profileService = profileService;
        _profiles = profiles;
    }

    public override async Task RunAsync(EftHttpContext context, GameProfileNicknameChangeRequest request)
    {
        // TODO:
        // * validate nickname usage
        // -- seionmoya, 2024/08/28

        var result = _profileService.IsValidNickname(request.Nickname);
        // TODO: Find if there is a more proper usage of EBackendErrorCode for this switch (find actual error message in globals.json)
        var errorMessage = result switch
        {
            ENicknameChangeResult.WrongSymbol => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.TooShort => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.CharacterLimit => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.InvalidNickname => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.NicknameTaken => EBackendErrorCode.NicknameNotUnique,
            ENicknameChangeResult.NicknameChangeTimeout => EBackendErrorCode.NicknameChangeTimeout,
            ENicknameChangeResult.DigitsLimit => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.Ok => EBackendErrorCode.None,
            _ => EBackendErrorCode.None,
        };

        if (result == ENicknameChangeResult.Ok)
        {
            //TODO: Save profile properly, currently doesn't persist?
            var profile = await _profiles.GetActiveProfileAsync(context.SessionId);

            profile.Pmc.Info.Nickname = request.Nickname;
            profile.Pmc.Info.LowerNickname = request.Nickname.ToLower();
            //profile.Pmc.Info.NicknameChangeDate = ???

            await _profiles.AddOrUpdateAsync(profile);
        }

        var response = new ResponseBody<GameProfileNicknameChangeResponse>()
        {
            err = (int)errorMessage,
            errmsg = errorMessage != EBackendErrorCode.None ? errorMessage.ToString() : null,
            data = new GameProfileNicknameChangeResponse()
            {
                Status = result
            }
        };

        var text = Json.Stringify(response);
        await context.SendJsonAsync(text, true, true);
    }
}