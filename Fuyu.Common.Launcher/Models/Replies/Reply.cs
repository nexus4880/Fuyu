using System.Runtime.Serialization;

namespace Fuyu.Common.Launcher.Models.Replies;

[DataContract]
public class Reply
{
    [DataMember(Name = "type")]
    public string Type { get; set; }
}