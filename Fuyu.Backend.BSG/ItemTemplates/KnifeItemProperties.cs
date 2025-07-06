using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class KnifeItemProperties : ItemProperties
{
    [DataMember(Name = "knifeDurab")]
    public float knifeDurab;

    [DataMember(Name = "MaxResource")]
    public float MaxResource;

    [DataMember(Name = "StimulatorBuffs")]
    public string StimulatorBuffs;

    [DataMember(Name = "knifeHitDelay")]
    public float knifeHitDelay;

    [DataMember(Name = "knifeHitSlashRate")]
    public float knifeHitSlashRate;

    [DataMember(Name = "knifeHitStabRate")]
    public float knifeHitStabRate;

    [DataMember(Name = "knifeHitRadius")]
    public float knifeHitRadius;

    [DataMember(Name = "knifeHitSlashDam")]
    public int knifeHitSlashDam;

    [DataMember(Name = "knifeHitStabDam")]
    public int knifeHitStabDam;

    [DataMember(Name = "PrimaryDistance")]
    public float PrimaryDistance;

    [DataMember(Name = "SecondryDistance")]
    public float SecondryDistance;

    [DataMember(Name = "StabPenetration")]
    public int StabPenetration;

    [DataMember(Name = "SlashPenetration")]
    public int SlashPenetration;

    [DataMember(Name = "PrimaryConsumption")]
    public float PrimaryConsumption;

    [DataMember(Name = "SecondryConsumption")]
    public float SecondryConsumption;

    [DataMember(Name = "DeflectionConsumption")]
    public float DeflectionConsumption;

    [DataMember(Name = "AppliedTrunkRotation")]
    // NOTE: Actually a Vector2
    // -- nexus4880, 2024-10-18
    public Vector3 AppliedTrunkRotation;

    [DataMember(Name = "AppliedHeadRotation")]
    // NOTE: Actually a Vector2
    // -- nexus4880, 2024-10-18
    public Vector3 AppliedHeadRotation;

    [DataMember(Name = "DisplayOnModel")]
    public bool DisplayOnModel;

    [DataMember(Name = "AdditionalAnimationLayer")]
    public int AdditionalAnimationLayer;

    [DataMember(Name = "StaminaBurnRate")]
    public float StaminaBurnRate;

    [DataMember(Name = "ColliderScaleMultiplier")]
    public Vector3 ColliderScaleMultiplier = new Vector3 { X = 1f, Y = 1f, Z = 1f };

    [DataMember(Name = "Durability")]
    public int Durability;

    [DataMember(Name = "MaxDurability")]
    public int MaxDurability;

    [DataMember(Name = "MinRepairDegradation")]
    public float MinRepairDegradation;

    [DataMember(Name = "MaxRepairDegradation")]
    public float MaxRepairDegradation;

    [DataMember(Name = "MinRepairKitDegradation")]
    public float MinRepairKitDegradation;

    [DataMember(Name = "MaxRepairKitDegradation")]
    public float MaxRepairKitDegradation;
}