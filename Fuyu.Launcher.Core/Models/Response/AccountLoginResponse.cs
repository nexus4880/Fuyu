using System.Runtime.Serialization;
using Fuyu.Launcher.Core.Models.Accounts;

namespace Fuyu.Launcher.Core.Models.Response;

[DataContract]
public class AccountLoginResponse
{
    [DataMember]
    public ELoginStatus Status { get; set; }

    [DataMember]
    public string SessionId { get; set; }
}