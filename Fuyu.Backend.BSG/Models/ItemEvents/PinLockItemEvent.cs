using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class PinLockItemEvent : BaseItemEvent
{
    [DataMember(Name = "Item")]
    public MongoId Item { get; set; }

    [DataMember(Name = "State")]
    public EPinLockState State { get; set; }
}

public enum EPinLockState
{
    Free,
    Pinned,
    Locked
}