using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class AmmoItemProperties : StackableItemItemProperties
{
    [DataMember(Name = "ammoType")]
    public string ammoType;

    [DataMember(Name = "Damage")]
    public int Damage;

    [DataMember(Name = "ammoAccr")]
    public int ammoAccr;

    [DataMember(Name = "ammoRec")]
    public int ammoRec;

    [DataMember(Name = "ammoDist")]
    public int ammoDist;

    [DataMember(Name = "buckshotBullets")]
    public int buckshotBullets;

    [DataMember(Name = "PenetrationPower")]
    public int PenetrationPower = 40;

    [DataMember(Name = "PenetrationPowerDiviation")]
    public float PenetrationPowerDiviation;

    [DataMember(Name = "ammoHear")]
    public int ammoHear;

    [DataMember(Name = "ammoSfx")]
    public string ammoSfx;

    [DataMember(Name = "MisfireChance")]
    public float MisfireChance;

    [DataMember(Name = "MinFragmentsCount")]
    public int MinFragmentsCount = 2;

    [DataMember(Name = "MaxFragmentsCount")]
    public int MaxFragmentsCount = 3;

    [DataMember(Name = "ammoShiftChance")]
    public int ammoShiftChance;

    [DataMember(Name = "casingName")]
    public string casingName;

    [DataMember(Name = "casingEjectPower")]
    public float casingEjectPower;

    [DataMember(Name = "casingMass")]
    public float casingMass;

    [DataMember(Name = "casingSounds")]
    public string casingSounds;

    [DataMember(Name = "ProjectileCount")]
    public int ProjectileCount = 1;

    [DataMember(Name = "InitialSpeed")]
    public float InitialSpeed = 700f;

    [DataMember(Name = "PenetrationDamageMod")]
    public float PenetrationDamageMod = 0.1f;

    [DataMember(Name = "PenetrationChanceObstacle")]
    public float PenetrationChanceObstacle = 0.2f;

    [DataMember(Name = "RicochetChance")]
    public float RicochetChance = 0.1f;

    [DataMember(Name = "FragmentationChance")]
    public float FragmentationChance = 0.03f;

    [DataMember(Name = "BallisticCoeficient")]
    public float BallisticCoeficient = 1f;

    [DataMember(Name = "Tracer")]
    public bool Tracer;

    [DataMember(Name = "TracerColor")]
    public ETaxonomyColor TracerColor;

    [DataMember(Name = "TracerDistance")]
    public float TracerDistance;

    [DataMember(Name = "ArmorDamage")]
    public int ArmorDamage;

    [DataMember(Name = "Caliber")]
    public string Caliber;

    [DataMember(Name = "StaminaBurnPerDamage")]
    public float StaminaBurnPerDamage;

    [DataMember(Name = "HasGrenaderComponent")]
    public bool HasGrenaderComponent;

    [DataMember(Name = "FuzeArmTimeSec")]
    public float FuzeArmTimeSec;

    [DataMember(Name = "MinExplosionDistance")]
    public float MinExplosionDistance;

    [DataMember(Name = "MaxExplosionDistance")]
    public float MaxExplosionDistance;

    [DataMember(Name = "FragmentsCount")]
    public int FragmentsCount;

    [DataMember(Name = "FragmentType")]
    public string FragmentType;

    [DataMember(Name = "ExplosionType")]
    public string ExplosionType;

    [DataMember(Name = "ShowHitEffectOnExplode")]
    public bool ShowHitEffectOnExplode;

    [DataMember(Name = "ExplosionStrength")]
    public float ExplosionStrength;

    [DataMember(Name = "ShowBullet")]
    public bool ShowBullet;

    [DataMember(Name = "AmmoLifeTimeSec")]
    public float AmmoLifeTimeSec = 2f;

    [DataMember(Name = "MalfMisfireChance")]
    public float MalfMisfireChance;

    [DataMember(Name = "MalfFeedChance")]
    public float MalfFeedChance;

    [DataMember(Name = "ArmorDistanceDistanceDamage")]
    public Vector3 ArmorDistanceDistanceDamage;

    [DataMember(Name = "Contusion")]
    public Vector3 Contusion;

    [DataMember(Name = "Blindness")]
    public Vector3 Blindness;

    [DataMember(Name = "LightBleedingDelta")]
    public float LightBleedingDelta;

    [DataMember(Name = "HeavyBleedingDelta")]
    public float HeavyBleedingDelta;

    [DataMember(Name = "IsLightAndSoundShot")]
    public bool IsLightAndSoundShot;

    [DataMember(Name = "LightAndSoundShotAngle")]
    public float LightAndSoundShotAngle;

    [DataMember(Name = "LightAndSoundShotSelfContusionTime")]
    public float LightAndSoundShotSelfContusionTime;

    [DataMember(Name = "LightAndSoundShotSelfContusionStrength")]
    public float LightAndSoundShotSelfContusionStrength;

    [DataMember(Name = "DurabilityBurnModificator")]
    public float DurabilityBurnModificator = 1f;

    [DataMember(Name = "HeatFactor")]
    public float HeatFactor = 1f;

    [DataMember(Name = "BulletMassGram")]
    public float BulletMassGram;

    [DataMember(Name = "BulletDiameterMilimeters")]
    public float BulletDiameterMilimeters;

    [DataMember(Name = "RemoveShellAfterFire")]
    public bool RemoveShellAfterFire;

    [DataMember(Name = "airDropTemplateId")]
    public string airDropTemplateId;

    [DataMember(Name = "FlareTypes")]
    public EFlareEventType[] FlareTypes;
}

public enum EFlareEventType
{
    Light,
    Airdrop,
    ExitActivate,
    Quest,
    AIFollowEvent,
    CallArtilleryOnMyself
}