using Fuyu.Platform.Common.Http;
using Fuyu.Platform.Common.IO;
using Fuyu.Platform.Common.Models.EFT.Responses;
using Fuyu.Platform.Common.Serialization;

namespace Fuyu.Platform.Server.Behaviours.EFT
{
    public class AchievementStatistic : FuyuBehaviour
    {
        private readonly ResponseBody<AchievementStatisticResponse> _response;

        public AchievementStatistic() : base("/client/achievement/statistic")
        {
            var json = Resx.GetText("eft", "database.eft.client.achievement.statistic.json");
            _response = Json.Parse<ResponseBody<AchievementStatisticResponse>>(json);
        }

        public override void Run(FuyuHttpContext context)
        {
            SendJson(context, Json.Stringify(_response));
        }
    }
}