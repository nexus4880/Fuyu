using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Survey;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class ClientSurveyOpinionRequest
{
    [DataMember(Name = "surveyId")]
    public int SurveyId { get; set; }

    [DataMember(Name = "answers")]
    public List<QuestionAnswer> Answers { get; set; }
}
