using System.Threading.Tasks;

namespace Fuyu.Common.Networking
{
    public class WsRouter : Router<WsController>
    {
        public WsRouter() : base()
        {
        }

        public async Task RouteAsync(WsContext context)
        {
            var matches = GetAllMatching(context);

            foreach (var match in matches)
            {
                await match.InitializeAsync(context);
                await Task.Run(() => match.RunAsync(context));
            }
        }
    }
}