using System.Runtime.Serialization;

namespace Fuyu.Launcher.Core.Models.Requests;

[DataContract]
public class AccountLoginRequest
{
    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public string Password { get; set; }
}