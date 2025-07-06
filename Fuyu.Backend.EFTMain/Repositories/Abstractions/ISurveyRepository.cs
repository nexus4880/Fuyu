using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Survey;

namespace Fuyu.Backend.EFTMain.Repositories.Abstractions;

public interface ISurveyRepository
{
    Task<SurveyResponse> GetSurveyAsync(int accountId);
    Task SurveyCompletedAsync(int aid, int surveyId, List<QuestionAnswer> answers);
}
