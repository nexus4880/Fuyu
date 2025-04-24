using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Common.Backend.Networking;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Networking;

public class ItemEventContext : IRouterContext
{
    public string SessionId { get; }
    public string Action { get; }
    public int RequestIndex { get; }
    public JToken Data { get; }
    public ItemEventResponse Response { get; }

    public ItemEventContext(string sessionId, string action, int requestIndex, JToken data, ItemEventResponse response)
    {
        SessionId = sessionId;
        Action = action;
        RequestIndex = requestIndex;
        Data = data;
        Response = response;
    }

    public void AppendInventoryError(string errorMessage, int code = 0)
    {
        Response.InventoryWarnings.Add(new InventoryWarning
        {
            ErrorCode = code.ToString(),
            ErrorMessage = errorMessage,
            RequestIndex = RequestIndex
        });
    }

    public T GetData<T>()
    {
        return Data.ToObject<T>();
    }

    public override string ToString()
    {
        return $"{GetType().Name}:{Action}({Data})";
    }
}