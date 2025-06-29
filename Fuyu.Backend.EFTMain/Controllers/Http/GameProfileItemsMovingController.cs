using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileItemsMovingController : AbstractEftHttpController<JObject>
{
    public ItemEventRouter ItemEventRouter { get; }
    private readonly EftOrm _eftOrm;

    public GameProfileItemsMovingController(IEnumerable<IItemEventController> controllers) : base("/client/game/profile/items/moving")
    {
        ItemEventRouter = new ItemEventRouter(controllers);
        _eftOrm = EftOrm.Instance;
    }

    public override async Task RunAsync(EftHttpContext context, JObject request)
    {
        if (!request.ContainsKey("data"))
        {
            return;
        }

        var sessionId = context.SessionId;
        var profile = _eftOrm.GetActiveProfile(sessionId);
        var requestData = request.Value<JArray>("data");
        var itemEventResponse = new ItemEventResponse();
        /*{
				
				ProfileChanges = {
					// NOTE: Possibly make this a method where we can do
					// context.GetProfileChange() and if it doesn't exist add it
					// -- nexus4880, 2024-10-22
					{ profile.Pmc._id, new ProfileChange() },
					{ profile.Savage._id, new ProfileChange() }
				},
				InventoryWarnings = []
				
			};*/

        itemEventResponse.ProfileChanges[profile.Pmc._id] = new ProfileChange();
        itemEventResponse.ProfileChanges[profile.Savage._id] = new ProfileChange();

        var requestIndex = 0;
        Exception ex = null;

        foreach (var itemRequest in requestData)
        {
            var action = itemRequest.Value<string>("Action");
            var itemEventContext = new ItemEventContext(sessionId, action, requestIndex, itemRequest, itemEventResponse);

            try
            {
                await ItemEventRouter.RouteAsync(itemEventContext);
            }
            catch (Exception innerException)
            {
                ex = innerException;
                break;
            }

            requestIndex++;
        }

        var response = new ResponseBody<ItemEventResponse>
        {
            data = itemEventResponse,
            err = ex != null ? 200 : 0,
            errmsg = ex?.Message
        };

        await context.SendResponseAsync(response, true, true);
    }
}