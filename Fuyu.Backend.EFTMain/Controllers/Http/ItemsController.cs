using System.Threading.Tasks;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.IO;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ItemsController : AbstractEftHttpController
{
    private readonly IItemTemplateRepository _itemTemplates;

    public ItemsController(IItemTemplateRepository itemTemplates) : base("/client/items")
    {
        _itemTemplates = itemTemplates;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var txt = Resx.GetText("eft", "database.client.items.json");
        var data = JObject.Parse(txt);
        await context.SendResponseAsync(new BSG.Models.Responses.ResponseBody<JObject> { data = data }, true, true);
    }
}