using System.Runtime.Serialization;
using Fuyu.Common.Launcher.Models.Replies;

namespace Fuyu.Launcher.Core.Models.Replies;

[DataContract]
public class LoginAccountReply : Reply
{
    [DataMember(Name = "message")]
    public string Message { get; set; }
}