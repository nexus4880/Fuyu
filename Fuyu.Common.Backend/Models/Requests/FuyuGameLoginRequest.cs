using System.Runtime.Serialization;

namespace Fuyu.Common.Backend.Models.Requests;

[DataContract]
public class FuyuGameLoginRequest
{
    [DataMember]
    public int AccountId { get; set; }
}