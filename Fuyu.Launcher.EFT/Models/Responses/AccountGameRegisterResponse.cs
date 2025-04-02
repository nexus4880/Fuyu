using System.Runtime.Serialization;

namespace Fuyu.Launcher.EFT.Models.Responses;

[DataContract]
public class AccountGameRegisterResponse
{
    [DataMember]
    public int AccountId { get; set; }
}