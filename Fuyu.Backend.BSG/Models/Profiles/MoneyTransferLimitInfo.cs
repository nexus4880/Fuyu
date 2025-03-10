using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class MoneyTransferLimitInfo
{
    [DataMember]
    public int nextResetTime { get; set; }

    [DataMember]
    public int remainingLimit { get; set; }

    [DataMember]
    public int totalLimit { get; set; }

    [DataMember]
    public int resetInterval { get; set; }
}