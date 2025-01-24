using System.Runtime.Serialization;
using Fuyu.Backend.Core.Models.Accounts;

namespace Fuyu.Backend.Core.Models.Responses;

[DataContract]
public class AccountRegisterResponse
{
    [DataMember]
    public ERegisterStatus Status { get; set; }
}