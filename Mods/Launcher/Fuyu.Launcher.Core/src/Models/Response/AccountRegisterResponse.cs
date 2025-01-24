using System.Runtime.Serialization;
using Fuyu.Launcher.Core.Models.Accounts;

namespace Fuyu.Launcher.Core.Models.Responses;

[DataContract]
public class AccountRegisterResponse
{
    [DataMember]
    public ERegisterStatus Status { get; set; }
}