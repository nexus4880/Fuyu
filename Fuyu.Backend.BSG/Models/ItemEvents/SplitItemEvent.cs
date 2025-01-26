using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class SplitItemEvent : BaseItemEvent
{
    [DataMember(Name = "splitItem")]
    public MongoId SplitItem { get; set; }

    [DataMember(Name = "newItem")]
    public MongoId NewItem { get; set; }

    [DataMember(Name = "container")]
    public RelocateTarget Container { get; set; }

    [DataMember(Name = "count")]
    public int Count { get; set; }
}