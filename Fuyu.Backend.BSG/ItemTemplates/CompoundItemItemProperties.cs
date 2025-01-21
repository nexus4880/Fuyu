using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class CompoundItemItemProperties : ItemProperties
{
    [DataMember(Name = "Grids")]
    public List<Grid> Grids = [];

    [DataMember(Name = "Slots")]
    public List<Slot> Slots = [];

    [DataMember(Name = "CanPutIntoDuringTheRaid")]
    public bool CanPutIntoDuringTheRaid;

    [DataMember(Name = "CantRemoveFromSlotsDuringRaid")]
    public List<EEquipmentSlot> CantRemoveFromSlotsDuringRaid = [];

    [DataMember(Name = "ForbidMissingVitalParts")]
    public bool ForbidMissingVitalParts;

    [DataMember(Name = "ForbidNonEmptyContainers")]
    public bool ForbidNonEmptyContainers;
}
