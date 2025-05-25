using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Survey;

[DataContract]
public class Question
{
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "sort")]
    public int Sort { get; set; }

    [DataMember(Name = "titleLocaleKey")]
    public string TitleLocaleKey { get; set; }

    [DataMember(Name = "hintLocaleKey")]
    public string HintLocaleKey { get; set; }

    /// <summary>
    /// If <see cref="AnswerType"/> is <see cref="EAnswerType.MultiOption"/> then
    /// this is the maximum amount of answers they can supply to that question, if
    /// it is <see cref="EAnswerType.Text"/> then this is the character limit
    /// </summary>
    // -- nexus4880, 2025-5-24
    [DataMember(Name = "answerLimit")]
    public int AnswerLimit { get; set; }

    [DataMember(Name = "answerType")]
    public EAnswerType AnswerType { get; set; }

    [DataMember(Name = "answers")]
    public List<Answer> Answers { get; set; }
}