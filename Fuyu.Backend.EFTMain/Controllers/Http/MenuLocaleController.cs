using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public partial class MenuLocaleController : AbstractEftHttpController
{
    [GeneratedRegex("^/client/menu/locale/(?<languageId>[a-z]+(-[a-z]+)?)$")]
    private static partial Regex PathExpression();

    private readonly IGameDataRepository _gameData;

    public MenuLocaleController(IGameDataRepository gameData) : base(PathExpression())
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var parameters = context.GetPathParameters(this);

        var languageId = parameters["languageId"];
        var locale = await _gameData.GetMenuLocaleAsync(languageId);
        var response = new ResponseBody<MenuLocaleResponse>
        {
            data = locale
        };

        await context.SendResponseAsync(response, true, true);
    }
}