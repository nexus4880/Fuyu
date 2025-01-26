using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class Slot
{
    [DataMember(Name = "_name")]
    public string Name { get; set; }

    [DataMember(Name = "_id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "_parent")]
    public MongoId Parent { get; set; }

    [DataMember(Name = "_props")]
    public SlotProperties Properties { get; set; }

    [DataMember(Name = "_required")]
    public bool Required { get; set; }

    [DataMember(Name = "_mergeSlotWithChildren")]
    public bool MergeSlotWithChildren { get; set; }

    [DataMember(Name = "_proto")]
    // I am hesitant to mark this as MongoId even though it seemingly is
    // because I am not nor have I ever been sure of what proto even is
    // -- nexus4880, 2024-10-18
    public string Proto { get; set; }
}