using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class SearchOtherProfileController : AbstractEftHttpController<SearchOtherProfileRequest>
{
    private readonly IProfileRepository _profiles;
    private readonly IAccountRepository _accounts;

    public SearchOtherProfileController(IProfileRepository profiles, IAccountRepository accounts) : base("/client/game/profile/search")
    {
        _profiles = profiles;
        _accounts = accounts;
    }

    public override async Task RunAsync(EftHttpContext context, SearchOtherProfileRequest body)
    {
        var activeProfile = await _accounts.GetBySessionAsync(context.SessionId);
        var currentSession = activeProfile.CurrentSession;

        if (!currentSession.HasValue)
        {
            throw new Exception("SessionMode is missing");
        }

        var profiles = new List<SearchOtherProfileResponse>();
        var allAccounts = await _accounts.GetAllAsync();
        foreach (var account in allAccounts)
        {
            var targetProfileId = currentSession == ESessionMode.Regular ? account.PvpId : account.PveId;
            var profile = await _profiles.GetByIdAsync(targetProfileId);

            if (profile.Pmc == null) continue; // nullcheck in case profile hasn't created their PMC yet

            if (profile.Pmc.Info.Nickname.Contains(body.Nickname, StringComparison.CurrentCultureIgnoreCase))
            {
                profiles.Add(new SearchOtherProfileResponse()
                {
                    Id = profile.Pmc._id,
                    AccountId = profile.Pmc.aid,
                    Info = new BSG.Models.Profiles.OtherProfileInfo()
                    {
                        Nickname = profile.Pmc.Info.Nickname,
                        Level = profile.Pmc.Info.Level,
                        Side = profile.Pmc.Info.Side,
                        MemberCategory = profile.Pmc.Info.MemberCategory,
                        SelectedMemberCategory = profile.Pmc.Info.SelectedMemberCategory
                    }
                });
            }
        }

        var response = new ResponseBody<SearchOtherProfileResponse[]>()
        {
            data = [.. profiles]
        };

        await context.SendResponseAsync(response, true, true);
    }
}