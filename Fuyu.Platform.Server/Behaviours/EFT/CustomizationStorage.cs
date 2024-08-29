using Fuyu.Platform.Common.Http;
using Fuyu.Platform.Common.Models.EFT.Responses;
using Fuyu.Platform.Common.Serialization;
using Fuyu.Platform.Server.Databases;

namespace Fuyu.Platform.Server.Behaviours.EFT
{
    public class CustomizationStorage : FuyuBehaviour
    {
        public override void Run(FuyuContext context)
        {
            var sessionId = context.GetSessionId();
            var accountId = FuyuDatabase.Accounts.GetSession(sessionId);
            var account = FuyuDatabase.Accounts.GetAccount(accountId);

            // TODO: PVP-PVE STATE DETECTION
            var response = new ResponseBody<CustomizationStorageResponse>()
            {
                data = new CustomizationStorageResponse()
                {
                    _id = account.EftSave.PvE.Pmc._id,
                    suites = account.EftSave.PvE.Suites
                }
            };

            SendJson(context, Json.Stringify(response));
        }
    }
}