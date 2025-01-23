using System.Runtime.Serialization;
using Fuyu.Common.Launcher.Models.Replies;

namespace Fuyu.Launcher.EFT.Models.Replies;

[DataContract]
public class LaunchGameReply : Reply
{
    [DataMember(Name = "error")]
    public string Message { get; set; }
}