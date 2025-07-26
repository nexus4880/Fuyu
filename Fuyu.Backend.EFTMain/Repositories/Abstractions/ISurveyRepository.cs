using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Survey;

namespace Fuyu.Backend.EFTMain.Repositories.Abstractions;

public interface ISurveyRepository
{
    Task<SurveyResponse> GetSurveyAsync(int accountId);
    Task ViewSurveyAsync(int aid, int surveyId);
    Task CompleteSurveyAsync(int aid, int surveyId, List<QuestionAnswer> answers);
}