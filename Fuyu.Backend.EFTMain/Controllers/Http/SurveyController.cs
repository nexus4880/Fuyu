using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class SurveyController : AbstractEftHttpController
{
    private readonly SurveyOrm _surveyOrm;

    public SurveyController() : base("/client/survey")
    {
        _surveyOrm = SurveyOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var activeSurvey = _surveyOrm.GetSurveyTemplate();
        var response = CreateSurveyResponse(activeSurvey);

        return context.SendResponseAsync(new ResponseBody<SurveyResponse> { data = response }, true, true);
    }

    private static SurveyResponse CreateSurveyResponse(SurveyTemplate template)
    {

        var locale = new Dictionary<string, Dictionary<string, string>>();
        var enLocale = locale["en"] = [];

        enLocale[template.WelcomePageData.TitleLocaleKey] = template.WelcomePageData.TitleLocaleKey;
        enLocale[template.WelcomePageData.TimeLocaleKey] = template.WelcomePageData.TimeLocaleKey;
        enLocale[template.WelcomePageData.DescriptionLocaleKey] = template.WelcomePageData.DescriptionLocaleKey;

        enLocale[template.FarewellPageData.TextLocaleKey] = template.FarewellPageData.TextLocaleKey;

        foreach (var question in template.Questions)
        {
            if (!string.IsNullOrEmpty(question.HintLocaleKey))
            {
                enLocale[question.HintLocaleKey] = question.HintLocaleKey;
            }

            if (!string.IsNullOrEmpty(question.TitleLocaleKey))
            {
                enLocale[question.TitleLocaleKey] = question.TitleLocaleKey;
            }

            foreach (var answer in question.Answers)
            {
                if (!string.IsNullOrEmpty(answer.LocaleKey))
                {
                    enLocale[answer.LocaleKey] = answer.LocaleKey;
                }
            }
        }

        return new SurveyResponse
        {
            Localization = locale,
            Template = template
        };
    }
}