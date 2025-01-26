using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class MergeItemEvent : BaseItemEvent
{
    [DataMember(Name = "item")]
    public MongoId Item { get; set; }

    [DataMember(Name = "with")]
    public MongoId With { get; set; }
}
