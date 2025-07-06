using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class GetOtherProfileRequest
{
    [DataMember(Name = "accountId")]
    public int AccountId { get; set; }
}