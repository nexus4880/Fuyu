using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Common.IO;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientSurveyOpinionController : AbstractEftHttpController<ClientSurveyOpinionRequest>
{
    private readonly ILogger<ClientSurveyOpinionController> _logger;
    private readonly IProfileRepository _profiles;
    private readonly IAccountRepository _accounts;
    private readonly SurveyOrm _surveyOrm;

    public ClientSurveyOpinionController(
        ILogger<ClientSurveyOpinionController> logger,
        IProfileRepository profiles,
        IAccountRepository accounts) : base("/client/survey/opinion")
    {
        _logger = logger;
        _accounts = accounts;
        _profiles = profiles;
        _surveyOrm = SurveyOrm.Instance;
    }

    public override async Task RunAsync(EftHttpContext context, ClientSurveyOpinionRequest body)
    {
        var profile = await _profiles.GetActiveProfileAsync(context.SessionId);
        var account = await _accounts.GetByIdAsync(profile.Pmc.aid);
        var completionLog = new StringBuilder();
        completionLog.AppendLine($"{account.Username} has completed the survey");
        var surveyTemplate = _surveyOrm.GetSurveyTemplate();

        foreach (var answer in body.Answers)
        {
            var questionAnswered = surveyTemplate.Questions.Find(q => q.Id == answer.QuestionId);
            completionLog.AppendLine($"Question: {questionAnswered.TitleLocaleKey}");

            switch (answer.AnswerType)
            {
                case EAnswerType.MultiOption:
                    {
                        var indexes = answer.Answers.Value.Value3;
                        var selectedAnswers = questionAnswered.Answers
                            .Where((value, index) => indexes.Contains(index))
                            .ToList();
                        completionLog.AppendLine($"Answer: {string.Join(", ", selectedAnswers.Select(answer => answer.LocaleKey))}");
                        break;
                    }
                case EAnswerType.Text:
                    {
                        completionLog.AppendLine($"Answer: {answer.Answers.Value.Value2.Trim()}");
                        break;
                    }
                case EAnswerType.SingleOption:
                    {
                        // If we abstained from voting or couldn't find the
                        // answer then create a new one with error text
                        // -- nexus4880, 2025-5-15
                        var index = answer.Answers.Value.Value1;
                        var selectedAnswer = index.HasValue && index >= 0 && index < questionAnswered.Answers.Count
                            ? questionAnswered.Answers[index.Value]
                            : new Answer { LocaleKey = $"Unknown, integer value was: {index}" };
                        completionLog.AppendLine($"Answer: {selectedAnswer.LocaleKey}");
                        break;
                    }
            }
        }

        _logger.LogInformation("{Log}", completionLog.ToString());
    }
}