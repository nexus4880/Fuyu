using System;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Backend.EFTMain.Databases;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Loaders;

public class SurveyLoader
{
    public static SurveyLoader Instance => instance.Value;
    private static readonly Lazy<SurveyLoader> instance = new(() => new SurveyLoader());

    private readonly SurveyDatabase _surveyDatabase;

    public SurveyLoader()
    {
        _surveyDatabase = SurveyDatabase.Instance;
    }

    public void Load()
    {
        var surveyJson = Resx.GetText("eft", "example_survey.json");
        var surveyTemplate = Json.Parse<SurveyTemplate>(surveyJson);
        _surveyDatabase.SurveyTemplate.Set(surveyTemplate);
    }
}