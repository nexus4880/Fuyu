using System.Runtime.Serialization;

namespace Fuyu.Common.Backend.Models.Responses;

[DataContract]
public class FuyuGameRegisterResponse
{
    [DataMember]
    public int AccountId;
}