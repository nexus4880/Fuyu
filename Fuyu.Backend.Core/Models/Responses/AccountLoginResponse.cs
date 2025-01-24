using System.Runtime.Serialization;
using Fuyu.Backend.Core.Models.Accounts;

namespace Fuyu.Backend.Core.Models.Responses;

[DataContract]
public class AccountLoginResponse
{
    [DataMember]
    public ELoginStatus Status { get; set; }

    [DataMember]
    public string SessionId { get; set; }
}