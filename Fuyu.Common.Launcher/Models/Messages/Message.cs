using System.Runtime.Serialization;

namespace Fuyu.Common.Launcher.Models.Messages;

[DataContract]
public class Message
{
    [DataMember(Name = "type")]
    public string Type { get; set; }
}