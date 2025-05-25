using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientSurveyViewController : AbstractEftHttpController<ClientSurveyViewRequest>
{
    public ClientSurveyViewController() : base("/client/survey/view")
    {
    }

    public override Task RunAsync(EftHttpContext context, ClientSurveyViewRequest body)
    {
        var profile = EftOrm.Instance.GetActiveProfile(context.SessionId);
        var account = EftOrm.Instance.GetAccount(profile.Pmc.aid);
        Terminal.WriteLine($"{account.Username} has viewed survey {body.SurveyId}");

        return Task.CompletedTask;
    }
}
