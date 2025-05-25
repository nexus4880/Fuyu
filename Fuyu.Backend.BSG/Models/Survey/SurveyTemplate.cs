using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Survey;

[DataContract]
public class SurveyTemplate
{
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "isNew")]
    public bool IsNew { get; set; }

    [DataMember(Name = "welcomePageData")]
    public WelcomePageData WelcomePageData { get; set; }

    [DataMember(Name = "farewellPageData")]
    public FarewellPageData FarewellPageData { get; set; }

    [DataMember(Name = "pages")]
    public List<List<int>> Pages { get; set; }

    [DataMember(Name = "questions")]
    public List<Question> Questions { get; set; }
}