using System.Runtime.Serialization;

namespace Fuyu.Common.Backend.Models.Requests;

[DataContract]
public class FuyuGameRegisterRequest
{
    [DataMember]
    public string Username;

    [DataMember]
    public string Edition;
}