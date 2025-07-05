using System;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Backend.EFTMain.Databases;

namespace Fuyu.Backend.EFTMain.Orms;

public class SurveyOrm
{
    public static SurveyOrm Instance => _instance.Value;
    private static readonly Lazy<SurveyOrm> _instance = new Lazy<SurveyOrm>(() => new SurveyOrm());

    private readonly SurveyDatabase _surveyDatabase;

    public SurveyOrm()
    {
        _surveyDatabase = SurveyDatabase.Instance;
    }

    public SurveyTemplate GetSurveyTemplate()
    {
        return _surveyDatabase.SurveyTemplate.Get();
    }

    public void SetSurveyTemplate(SurveyTemplate surveyTemplate)
    {
        _surveyDatabase.SurveyTemplate.Set(surveyTemplate);
    }
}