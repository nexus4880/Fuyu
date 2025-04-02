using System.Runtime.Serialization;

namespace Fuyu.Launcher.EFT.Models.Responses;

[DataContract]
public class FuyuGameLoginResponse
{
    [DataMember]
    public string SessionId { get; set; }
}