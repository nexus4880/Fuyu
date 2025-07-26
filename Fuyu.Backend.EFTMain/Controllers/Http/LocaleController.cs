using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public partial class LocaleController : AbstractEftHttpController
{
    [GeneratedRegex("^/client/locale/(?<languageId>[a-z]+(-[a-z]+)?)$")]
    private static partial Regex PathExpression();

    private readonly IGameDataRepository _gameData;

    public LocaleController(IGameDataRepository gameData) : base(PathExpression())
    {
        _gameData = gameData;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var parameters = context.GetPathParameters(this);

        var languageId = parameters["languageId"];
        var locale = await _gameData.GetGlobalLocaleAsync(languageId);
        var response = new ResponseBody<Dictionary<string, string>>
        {
            data = locale
        };

        await context.SendJsonAsync(Json.Stringify(response), true, true);
    }
}