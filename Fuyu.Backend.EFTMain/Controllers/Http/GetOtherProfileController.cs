using System;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GetOtherProfileController : AbstractEftHttpController<GetOtherProfileRequest>
{
    private readonly EftOrm _eftOrm;

    public GetOtherProfileController() : base("/client/profile/view")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, GetOtherProfileRequest body)
    {
        var activeProfile = _eftOrm.GetAccount(context.SessionId);
        var currentSession = activeProfile.CurrentSession;

        if (!currentSession.HasValue)
        {
            throw new Exception("SessionMode is missing");
        }

        var account = _eftOrm.GetAccount(body.AccountId);
        var targetProfileId = currentSession == ESessionMode.Regular ? account.PvpId : account.PveId;
        var targetProfile = _eftOrm.GetProfile(targetProfileId);

        if (targetProfile == null)
        {
            throw new Exception($"Could not find ProfileId {targetProfileId}");
        }

        var favoriteItems = targetProfile.Pmc.Inventory.Items.Where(x => targetProfile.Pmc.Inventory.FavoriteItems.Contains(x.Id)).ToArray();

        var response = new ResponseBody<GetOtherProfileResponse>()
        {
            data = new GetOtherProfileResponse()
            {
                AccountId = targetProfile.Pmc.aid,
                Info = new PlayerInfo()
                {
                    Nickname = targetProfile.Pmc.Info.Nickname,
                    Side = targetProfile.Pmc.Info.Side.ToString(),
                    Experience = (int)targetProfile.Pmc.Info.Experience,
                    MemberCategory = targetProfile.Pmc.Info.MemberCategory,
                    SelectedMemberCategory = targetProfile.Pmc.Info.SelectedMemberCategory,
                    PrestigeLevel = targetProfile.Pmc.Info.PrestigeLevel,
                    RegistrationDate = (int)targetProfile.Pmc.Info.RegistrationDate
                },
                Achievements = targetProfile.Pmc.Achievements,
                Customization = targetProfile.Pmc.Customization,
                FavoriteItems = favoriteItems,
                Equipment = new ItemInfo()
                {
                    Id = targetProfile.Pmc.Inventory.Equipment,
                    Items = targetProfile.Pmc.Inventory.Items
                },
                PmcStats = targetProfile.Pmc.Stats.ToSimpleStatsInfo(),
                ScavStats = targetProfile.Savage.Stats.ToSimpleStatsInfo(),
                Skills = targetProfile.Pmc.Skills
                // TODO: Fix hideout data, our models and implementation are not completed so this data cannot be filled
                // -- Lacyway 2025-01-12
                /*Hideout = targetProfile.Pmc.Hideout,
                Items = targetProfile.Pmc.Hideout.AllItemsInSlots,
                HideoutAreaStashes = []*/
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}