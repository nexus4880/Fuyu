using System.Runtime.Serialization;

namespace Fuyu.Launcher.EFT.Models.Requests;

[DataContract]
public class AccountGameRegisterRequest
{
    [DataMember]
    public string Game { get; set; }

    [DataMember]
    public string Edition { get; set; }
}