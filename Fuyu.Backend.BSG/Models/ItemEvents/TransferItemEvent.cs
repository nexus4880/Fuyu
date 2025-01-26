using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class TransferItemEvent : BaseItemEvent
{
    [DataMember(Name = "item")]
    public MongoId Item { get; set; }

    [DataMember(Name = "with")]
    public MongoId With { get; set; }

    [DataMember(Name = "count")]
    public int Count { get; set; }
}