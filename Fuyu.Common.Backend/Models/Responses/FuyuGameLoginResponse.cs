using System.Runtime.Serialization;

namespace Fuyu.Common.Backend.Models.Responses;

[DataContract]
public class FuyuGameLoginResponse
{
    [DataMember]
    public string SessionId;
}