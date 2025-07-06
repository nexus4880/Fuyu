using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class ArmoredEquipmentItemProperties : EquipmentItemProperties
{
    [DataMember(Name = "FaceShieldComponent")]
    public bool FaceShieldComponent;

    [DataMember(Name = "HasHinge")]
    public bool HasHinge;

    [DataMember(Name = "Durability")]
    public int Durability;

    [DataMember(Name = "MaxDurability")]
    public int MaxDurability;

    [DataMember(Name = "armorClass")]
    public int armorClass;

    [DataMember(Name = "speedPenaltyPercent")]
    public float speedPenaltyPercent;

    [DataMember(Name = "mousePenalty")]
    public float mousePenalty;

    [DataMember(Name = "weaponErgonomicPenalty")]
    public float weaponErgonomicPenalty;

    [DataMember(Name = "MaterialType")]
    public EMaterialType MaterialType;

    [DataMember(Name = "ArmorType")]
    public EArmorType ArmorType;

    [DataMember(Name = "BluntThroughput")]
    public float BluntThroughput;

    [DataMember(Name = "ArmorMaterial")]
    public EArmorMaterial ArmorMaterial;

    [DataMember(Name = "RicochetParams")]
    public Vector3 RicochetParams;

    [DataMember(Name = "DeafStrength")]
    public EDeafStrength DeafStrength;

    [DataMember(Name = "armorColliders")]
    public EBodyPartColliderType[] armorColliders = [];

    [DataMember(Name = "armorPlateColliders")]
    public EArmorPlateCollider[] armorPlateColliders = [];

    [DataMember(Name = "FaceShieldMask")]
    public EFaceShieldMask FaceShieldMask;

    [DataMember(Name = "BlindnessProtection")]
    public float BlindnessProtection;
}

public enum EFaceShieldMask
{
    NoMask,
    Narrow,
    Wide
}