using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class ArmorPlateItemProperties : ArmoredEquipmentItemProperties
{
    [DataMember(Name = "Lower75Prefab")]
    public ResourceKey Lower75Prefab;

    [DataMember(Name = "Lower40Prefab")]
    public ResourceKey Lower40Prefab;
}