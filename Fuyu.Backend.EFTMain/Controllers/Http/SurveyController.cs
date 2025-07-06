using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class SurveyController : AbstractEftHttpController
{
    private readonly ISessionRepository _sessions;
    private readonly ISurveyRepository _surveys;

    public SurveyController(ISessionRepository sessions, ISurveyRepository surveys) : base("/client/survey")
    {
        _sessions = sessions;
        _surveys = surveys;
    }

    public override async Task RunAsync(EftHttpContext context)
    {
        var aid = await _sessions.GetAccountIdAsync(context.SessionId);
        var activeSurvey = await _surveys.GetSurveyAsync(aid);
        var response = new ResponseBody<SurveyResponse>
        {
            data = activeSurvey
        };

        await context.SendResponseAsync(response, true, true);
    }
}