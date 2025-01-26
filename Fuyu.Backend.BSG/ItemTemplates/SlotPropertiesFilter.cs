using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class SlotPropertiesFilter
{
    [DataMember(Name = "Filter")]
    public List<MongoId> Filter { get; set; }

    [DataMember(Name = "Shift", EmitDefaultValue = false)]
    public int? Shift { get; set; }

    [DataMember(Name = "Plate", EmitDefaultValue = false)]
    public MongoId? Plate { get; set; }

    [DataMember(Name = "armorColliders", EmitDefaultValue = false)]
    public List<string> ArmorColliders { get; set; }

    [DataMember(Name = "armorPlateColliders", EmitDefaultValue = false)]
    public List<string> ArmorPlateColliders { get; set; }

    [DataMember(Name = "bluntDamageReduceFromSoftArmor", EmitDefaultValue = false)]
    public bool? BluntDamageReduceFromSoftArmor { get; set; }

    [DataMember(Name = "locked", EmitDefaultValue = false)]
    public bool? Locked { get; set; }
}