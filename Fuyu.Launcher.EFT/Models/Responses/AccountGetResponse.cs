using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Launcher.EFT.Models.Responses;

[DataContract]
public class AccountGetResponse
{
    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public Dictionary<string, int?> Games { get; set; }
}