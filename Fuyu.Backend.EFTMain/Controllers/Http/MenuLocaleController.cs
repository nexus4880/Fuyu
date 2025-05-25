using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public partial class MenuLocaleController : AbstractEftHttpController
{
    [GeneratedRegex("^/client/menu/locale/(?<languageId>[a-z]+(-[a-z]+)?)$")]
    private static partial Regex PathExpression();
    private readonly EftOrm _eftOrm;

    public MenuLocaleController() : base(PathExpression())
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var parameters = context.GetPathParameters(this);

        var languageId = parameters["languageId"];
        var locale = _eftOrm.GetMenuLocale(languageId);
        var response = new ResponseBody<MenuLocaleResponse>
        {
            data = locale
        };

        return context.SendResponseAsync(response, true, true);
    }
}