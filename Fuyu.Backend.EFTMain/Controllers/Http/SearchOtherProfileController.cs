using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class SearchOtherProfileController : AbstractEftHttpController<SearchOtherProfileRequest>
{
    private readonly EftOrm _eftOrm;

    public SearchOtherProfileController() : base("/client/game/profile/search")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, SearchOtherProfileRequest body)
    {
        var activeProfile = _eftOrm.GetAccount(context.SessionId);
        var currentSession = activeProfile.CurrentSession;

        if (!currentSession.HasValue)
        {
            throw new Exception("SessionMode is missing");
        }

        var profiles = new List<SearchOtherProfileResponse>();
        foreach (var account in _eftOrm.GetAccounts())
        {
            var targetProfileId = currentSession == ESessionMode.Regular ? account.PvpId : account.PveId;
            var profile = _eftOrm.GetProfile(targetProfileId);

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

        return context.SendResponseAsync(response, true, true);
    }
}