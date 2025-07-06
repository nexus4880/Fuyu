using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientSurveyOpinionController : AbstractEftHttpController<ClientSurveyOpinionRequest>
{
    private readonly IProfileRepository _profiles;
    private readonly IAccountRepository _accounts;
    private readonly ISurveyRepository _surveys;
    private readonly ISessionRepository _sessions;

    public ClientSurveyOpinionController(
        IProfileRepository profiles,
        IAccountRepository accounts,
        ISessionRepository sessions,
        ISurveyRepository surveys
        ) : base("/client/survey/opinion")
    {
        _accounts = accounts;
        _profiles = profiles;
        _sessions = sessions;
        _surveys = surveys;
    }

    public override async Task RunAsync(EftHttpContext context, ClientSurveyOpinionRequest body)
    {
        var aid = await _sessions.GetAccountIdAsync(context.SessionId);
        await _surveys.SurveyCompletedAsync(aid, body.SurveyId, body.Answers);
    }
}