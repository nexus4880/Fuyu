using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Backend.Core.Models.Responses;

[DataContract]
public class AccountGetResponse
{
    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public Dictionary<string, int?> Games { get; set; }
}