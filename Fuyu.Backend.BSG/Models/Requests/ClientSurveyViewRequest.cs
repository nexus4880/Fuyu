using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class ClientSurveyViewRequest
{
    [DataMember(Name = "surveyId")]
    public int SurveyId { get; set; }
}