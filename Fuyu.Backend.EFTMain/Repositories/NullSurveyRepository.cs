using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Repositories;

/// <summary>
/// Does not return a survey and does nothing on completion/view
/// </summary>
public class NullSurveyRepository : ISurveyRepository
{
    public Task<SurveyResponse> GetSurveyAsync(int accountId)
    {
        return Task.FromResult<SurveyResponse>(null);
    }

    public Task CompleteSurveyAsync(int aid, int surveyId, List<QuestionAnswer> answers)
    {
        return Task.CompletedTask;
    }

    public Task ViewSurveyAsync(int aid, int surveyId)
    {
        return Task.CompletedTask;
    }
}
