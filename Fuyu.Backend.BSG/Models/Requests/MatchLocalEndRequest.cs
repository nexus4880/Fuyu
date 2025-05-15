using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Models.Raid;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class MatchLocalEndRequest
{
    [DataMember(Name = "serverId")]
    public string ServerId { get; set; }

    [DataMember(Name = "results")]
    public MatchLocalEndResult MatchEndResult { get; set; }

    [DataMember(Name = "lostInsuredItems")]
    public List<ItemInstance> LostInsuredItems { get; set; }

    [DataMember(Name = "transferItems")]
    public Dictionary<string, List<ItemInstance>> TransferItems { get; set; }

    [DataMember(Name = "locationTransit")]
    public TransitData LocationTransit { get; set; }
}