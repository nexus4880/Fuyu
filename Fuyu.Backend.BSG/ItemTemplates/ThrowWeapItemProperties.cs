using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class ThrowWeapItemProperties : ItemProperties
{
    [DataMember(Name = "ThrowType")]
    public EThrowWeapType ThrowType;

    [DataMember(Name = "ExplDelay")]
    public float ExplDelay;

    [DataMember(Name = "MinExplosionDistance")]
    public float MinExplosionDistance;

    [DataMember(Name = "MaxExplosionDistance")]
    public float MaxExplosionDistance;

    [DataMember(Name = "FragmentsCount")]
    public int FragmentsCount;

    [DataMember(Name = "MinFragmentDamage")]
    public float MinFragmentDamage;

    [DataMember(Name = "MaxFragmentDamage")]
    public float MaxFragmentDamage;

    [DataMember(Name = "Strength")]
    public float Strength;

    [DataMember(Name = "FragmentType")]
    public string FragmentType;

    [DataMember(Name = "Blindness")]
    public Vector3 Blindness;

    [DataMember(Name = "Contusion")]
    public Vector3 Contusion;

    [DataMember(Name = "EmitTime")]
    public int EmitTime;

    [DataMember(Name = "CanBeHiddenDuringThrow")]
    public bool CanBeHiddenDuringThrow;

    [DataMember(Name = "ArmorDistanceDistanceDamage")]
    public Vector3 ArmorDistanceDistanceDamage;

    [DataMember(Name = "MinTimeToContactExplode")]
    public float MinTimeToContactExplode = -1f;

    [DataMember(Name = "ExplosionEffectType")]
    public string ExplosionEffectType;

    [DataMember(Name = "CanPlantOnGround")]
    public bool CanPlantOnGround;
}

public enum EThrowWeapType
{
    frag_grenade,
    flash_grenade,
    stun_grenade,
    smoke_grenade,
    gas_grenade,
    incendiary_grenade,
    sonar
}