using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Launcher.EFT.Models.Responses;

[DataContract]
public class AccountGamesResponse
{
    [DataMember]
    public Dictionary<string, int?> Games { get; set; }
}