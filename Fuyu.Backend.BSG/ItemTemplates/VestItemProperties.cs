using System;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class VestItemProperties : SearchableItemItemProperties
{
    [DataMember(Name = "Durability")]
    public int Durability;

    [DataMember(Name = "MaxDurability")]
    public int MaxDurability;

    [DataMember(Name = "RigLayoutName")]
    public string RigLayoutName;

    [DataMember(Name = "armorZone")]
    public EBodyPart[] armorZone;

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
    public EBodyPartColliderType[] armorColliders;

    [DataMember(Name = "armorPlateColliders")]
    public EArmorPlateCollider[] armorPlateColliders;
}

public enum EBodyPart
{
    Head,
    Chest,
    Stomach,
    LeftArm,
    RightArm,
    LeftLeg,
    RightLeg,
    Common
}

public enum EMaterialType
{
    Asphalt = 1,
    Body,
    Cardboard,
    Chainfence,
    Concrete,
    Fabric,
    GarbageMetal,
    GarbagePaper,
    GenericSoft,
    Glass,
    GlassShattered,
    Grate,
    GrassHigh,
    GrassLow,
    Gravel,
    MetalThin,
    MetalThick,
    Mud,
    Pebbles,
    Plastic,
    Stone,
    Soil,
    SoilForest,
    Tile,
    Water,
    WaterPuddle,
    WoodThin,
    WoodThick,
    Tyre,
    Rubber,
    GenericHard,
    BodyArmor,
    Swamp,
    Helmet,
    GlassVisor,
    HelmetRicochet,
    MetalNoDecal,
    Snow,
    None = 0
}

public enum EArmorType
{
    // Token: 0x0400C956 RID: 51542
    None,
    // Token: 0x0400C957 RID: 51543
    Light,
    // Token: 0x0400C958 RID: 51544
    Heavy
}

public enum EArmorMaterial
{
    UHMWPE,
    Aramid,
    Combined,
    Titan,
    Aluminium,
    ArmoredSteel,
    Ceramic,
    Glass
}

public enum EDeafStrength : byte
{
    None,
    Low,
    High
}

public enum EBodyPartColliderType
{
    None = -1,
    HeadCommon,
    RibcageUp,
    Pelvis = 3,
    LeftUpperArm,
    LeftForearm,
    RightUpperArm,
    RightForearm,
    LeftThigh,
    LeftCalf,
    RightThigh,
    RightCalf,
    ParietalHead,
    BackHead,
    Ears,
    Eyes,
    Jaw,
    NeckFront,
    NeckBack,
    RightSideChestUp,
    LeftSideChestUp,
    SpineTop,
    SpineDown,
    PelvisBack,
    RightSideChestDown,
    LeftSideChestDown,
    RibcageLow
}

[Flags]
public enum EArmorPlateCollider : short
{
    Plate_Granit_SAPI_chest = 1,
    Plate_Granit_SAPI_back = 2,
    Plate_Granit_SSAPI_side_left_high = 4,
    Plate_Granit_SSAPI_side_left_low = 8,
    Plate_Granit_SSAPI_side_right_high = 16,
    Plate_Granit_SSAPI_side_right_low = 32,
    Plate_Korund_chest = 64,
    Plate_6B13_back = 128,
    Plate_Korund_side_left_high = 256,
    Plate_Korund_side_left_low = 512,
    Plate_Korund_side_right_high = 1024,
    Plate_Korund_side_right_low = 2048
}