using System.Runtime.Serialization;

namespace Fuyu.Backend.Core.Models.Responses;

[DataContract]
public class AccountGameRegisterResponse
{
    [DataMember]
    public int AccountId { get; set; }
}