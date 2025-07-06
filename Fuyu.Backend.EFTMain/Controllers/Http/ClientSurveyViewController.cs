using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientSurveyViewController : AbstractEftHttpController<ClientSurveyViewRequest>
{
    private readonly ISessionRepository _sessions;
    private readonly ISurveyRepository _surveys;

    public ClientSurveyViewController(ISessionRepository sessions, ISurveyRepository surveys) : base("/client/survey/view")
    {
        _sessions = sessions;
        _surveys = surveys;
    }

    public override async Task RunAsync(EftHttpContext context, ClientSurveyViewRequest body)
    {
        var aid = await _sessions.GetAccountIdAsync(context.SessionId);
        await _surveys.ViewSurveyAsync(aid, body.SurveyId);
    }
}