using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;

namespace Fuyu.Backend.EFTMain.Repositories;

public class NullSurveyRepository : ISurveyRepository
{
    public Task<SurveyResponse> GetSurveyAsync(int accountId)
    {
        return Task.FromResult<SurveyResponse>(null);
    }

    public Task SurveyCompletedAsync(int aid, int surveyId, List<QuestionAnswer> answers)
    {
        return Task.CompletedTask;
    }
}
