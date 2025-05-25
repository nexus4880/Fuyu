using System;
using Fuyu.Backend.BSG.Models.Survey;
using Fuyu.Common.Collections;

namespace Fuyu.Backend.EFTMain.Databases;

public class SurveyDatabase
{
    public static SurveyDatabase Instance => _instance.Value;
    private static readonly Lazy<SurveyDatabase> _instance = new(() => new SurveyDatabase());

    public readonly ThreadObject<SurveyTemplate> SurveyTemplate;

    private SurveyDatabase()
    {
        SurveyTemplate = new ThreadObject<SurveyTemplate>(null);
    }
}