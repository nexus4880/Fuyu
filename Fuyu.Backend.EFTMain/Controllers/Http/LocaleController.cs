using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public partial class LocaleController : AbstractEftHttpController
{
    [GeneratedRegex("^/client/locale/(?<languageId>[a-z]+(-[a-z]+)?)$")]
    private static partial Regex PathExpression();
    private readonly EftOrm _eftOrm;

    public LocaleController() : base(PathExpression())
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var parameters = context.GetPathParameters(this);

        var languageId = parameters["languageId"];
        var locale = _eftOrm.GetGlobalLocale(languageId);
        var response = new ResponseBody<Dictionary<string, string>>
        {
            data = locale
        };

        return context.SendJsonAsync(Json.Stringify(response), true, true);
    }
}