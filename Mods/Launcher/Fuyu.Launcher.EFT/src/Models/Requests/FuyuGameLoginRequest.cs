using System.Runtime.Serialization;

namespace Fuyu.Launcher.EFT.Models.Requests;

[DataContract]
public class FuyuGameLoginRequest
{
    [DataMember]
    public int AccountId { get; set; }
}