using System.Runtime.Serialization;
using Fuyu.Common.Launcher.Models.Messages;

namespace Fuyu.Launcher.Core.Models.Messages;

[DataContract]
public class RegisterAccountMessage : Message
{
    [DataMember(Name = "username")]
    public string Username { get; set; }

    [DataMember(Name = "password")]
    public string Password { get; set; }
}