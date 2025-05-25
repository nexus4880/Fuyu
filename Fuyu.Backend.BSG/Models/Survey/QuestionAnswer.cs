using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Common.Collections;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Survey;

[DataContract]
public class QuestionAnswer
{
    [DataMember(Name = "questionId")]
    public int QuestionId { get; set; }

    [DataMember(Name = "answerType")]
    public EAnswerType AnswerType { get; set; }

    [DataMember(Name = "answers")]
    public AnswerObject Answers { get; set; }
}

[DataContract]
public class AnswerObject
{
    [DataMember(Name = "value")]
    [UnionMappings(JTokenType.Integer, JTokenType.String, JTokenType.Array)]
    public Union<int?, string, List<int>> Value { get; set; }
}
