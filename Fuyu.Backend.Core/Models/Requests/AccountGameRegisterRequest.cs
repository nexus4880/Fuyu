using System.Runtime.Serialization;

namespace Fuyu.Backend.Core.Models.Requests;

[DataContract]
public class AccountGameRegisterRequest
{
    [DataMember]
    public string Game { get; set; }

    [DataMember]
    public string Edition { get; set; }
}