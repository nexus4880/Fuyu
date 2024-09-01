using Fuyu.Platform.Common.Http;
using Fuyu.Platform.Common.IO;

namespace Fuyu.Platform.Server.Behaviours.EFT
{
    public class TraderSettings : FuyuBehaviour
    {
        private readonly string _response;

        public TraderSettings() : base("/client/trading/api/traderSettings")
        {
            _response = Resx.GetText("eft", "database.eft.client.trading.api.traderSettings.json");
        }

        public override void Run(FuyuHttpContext context)
        {
            SendJson(context, _response);
        }
    }
}