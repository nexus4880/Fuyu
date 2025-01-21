using System.Runtime.Serialization;
using Fuyu.Backend.BSG.DTO.Common;
using Fuyu.Backend.BSG.Models.Common;
using Fuyu.Common.Collections;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class WeaponItemProperties : CompoundItemItemProperties
{
    [DataMember(Name = "ReloadMode")]
    public EReloadMode ReloadMode;

    [DataMember(Name = "weapClass")]
    public string weapClass;

    [DataMember(Name = "weapUseType")]
    public string weapUseType;

    [DataMember(Name = "ammoCaliber")]
    public string ammoCaliber;

    [DataMember(Name = "AdjustCollimatorsToTrajectory")]
    public bool AdjustCollimatorsToTrajectory;

    [DataMember(Name = "weapAmmoTypes")]
    public object[] weapAmmoTypes;

    [DataMember(Name = "Durability")]
    public float Durability;

    [DataMember(Name = "MaxDurability")]
    public float MaxDurability;

    [DataMember(Name = "RepairComplexity")]
    public float RepairComplexity;

    [DataMember(Name = "OperatingResource")]
    public float OperatingResource;

    [DataMember(Name = "RecoilCategoryMultiplierHandRotation")]
    public float RecoilCategoryMultiplierHandRotation;

    [DataMember(Name = "RecoilReturnSpeedHandRotation")]
    public float RecoilReturnSpeedHandRotation;

    [DataMember(Name = "RecoilDampingHandRotation")]
    public float RecoilDampingHandRotation;

    [DataMember(Name = "RecoilCamera")]
    public float RecoilCamera;

    [DataMember(Name = "RecoilStableIndexShot")]
    public int RecoilStableIndexShot;

    [DataMember(Name = "RecoilForceBack")]
    public float RecoilForceBack;

    [DataMember(Name = "RecoilForceUp")]
    public float RecoilForceUp;

    [DataMember(Name = "RecolDispersion")]
    public int RecolDispersion;

    [DataMember(Name = "RecoilPosZMult")]
    public float RecoilPosZMult = 1f;

    [DataMember(Name = "RecoilReturnPathDampingHandRotation")]
    public float RecoilReturnPathDampingHandRotation;

    [DataMember(Name = "RecoilReturnPathOffsetHandRotation")]
    public float RecoilReturnPathOffsetHandRotation;

    [DataMember(Name = "RecoilAngle")]
    public int RecoilAngle;

    [DataMember(Name = "RecoilStableAngleIncreaseStep")]
    public float RecoilStableAngleIncreaseStep;

    [DataMember(Name = "ProgressRecoilAngleOnStable")]
    public Vector3 ProgressRecoilAngleOnStable;

    [DataMember(Name = "RecoilCenter")]
    public Vector3 RecoilCenter;

    [DataMember(Name = "PostRecoilVerticalRangeHandRotation")]
    public Vector2 PostRecoilVerticalRangeHandRotation;

    [DataMember(Name = "PostRecoilHorizontalRangeHandRotation")]
    public Vector2 PostRecoilHorizontalRangeHandRotation;

    [DataMember(Name = "CameraToWeaponAngleSpeedRange")]
    public Vector2 CameraToWeaponAngleSpeedRange;

    [DataMember(Name = "CameraToWeaponAngleStep")]
    public float CameraToWeaponAngleStep;

    [DataMember(Name = "ShotsGroupSettings")]
    public object[] ShotsGroupSettings;

    [DataMember(Name = "CameraSnap")]
    public float CameraSnap;

    [DataMember(Name = "MountingPosition")]
    public Vector3 MountingPosition;

    [DataMember(Name = "MountVerticalRecoilMultiplier")]
    public float MountVerticalRecoilMultiplier;

    [DataMember(Name = "MountHorizontalRecoilMultiplier")]
    public float MountHorizontalRecoilMultiplier;

    [DataMember(Name = "MountReturnSpeedHandMultiplier")]
    public float MountReturnSpeedHandMultiplier;

    [DataMember(Name = "MountCameraSnapMultiplier")]
    public float MountCameraSnapMultiplier;

    [DataMember(Name = "Ergonomics")]
    public float Ergonomics;

    [DataMember(Name = "Velocity")]
    public float Velocity;

    [DataMember(Name = "durabSpawnMin")]
    public int durabSpawnMin;

    [DataMember(Name = "durabSpawnMax")]
    public int durabSpawnMax;

    [DataMember(Name = "isFastReload")]
    public bool isFastReload;

    [DataMember(Name = "isChamberLoad")]
    public bool isChamberLoad;

    [DataMember(Name = "ShotgunDispersion")]
    public int ShotgunDispersion;

    [DataMember(Name = "bFirerate")]
    public int bFirerate;

    [DataMember(Name = "SingleFireRate")]
    public int SingleFireRate = 240;

    [DataMember(Name = "CanQueueSecondShot")]
    public bool CanQueueSecondShot = true;

    [DataMember(Name = "bEffDist")]
    public int bEffDist;

    [DataMember(Name = "bHearDist")]
    public int bHearDist;

    [DataMember(Name = "isBoltCatch")]
    public bool isBoltCatch;

    [DataMember(Name = "defMagType")]
    public string defMagType;

    [DataMember(Name = "defAmmo")]
    public string defAmmo;

    [DataMember(Name = "AimPlane")]
    public float AimPlane;

    [DataMember(Name = "Chambers")]
    public Slot[] Chambers;

    [DataMember(Name = "CenterOfImpact")]
    public float CenterOfImpact;

    [DataMember(Name = "DoubleActionAccuracyPenalty")]
    public float DoubleActionAccuracyPenalty;

    [DataMember(Name = "DeviationMax")]
    public float DeviationMax;

    [DataMember(Name = "DeviationCurve")]
    public float DeviationCurve;

    [DataMember(Name = "MustBoltBeOpennedForExternalReload")]
    public bool MustBoltBeOpennedForExternalReload;

    [DataMember(Name = "MustBoltBeOpennedForInternalReload")]
    public bool MustBoltBeOpennedForInternalReload;

    [DataMember(Name = "Foldable")]
    public bool Foldable;

    [DataMember(Name = "Retractable")]
    public bool Retractable;

    [DataMember(Name = "BoltAction")]
    public bool BoltAction;

    [DataMember(Name = "ManualBoltCatch")]
    public bool ManualBoltCatch;

    [DataMember(Name = "TacticalReloadStiffnes")]
    public Vector3 TacticalReloadStiffnes;

    [DataMember(Name = "TacticalReloadFixation")]
    public float TacticalReloadFixation;

    [DataMember(Name = "RotationCenter")]
    public Vector3 RotationCenter;

    [DataMember(Name = "RotationCenterNoStock")]
    public Vector3 RotationCenterNoStock;

    [DataMember(Name = "IronSightRange")]
    public int IronSightRange;

    [DataMember(Name = "HipInnaccuracyGain")]
    public float HipInnaccuracyGain;

    [DataMember(Name = "HipAccuracyRestorationDelay")]
    public float HipAccuracyRestorationDelay;

    [DataMember(Name = "HipAccuracyRestorationSpeed")]
    public float HipAccuracyRestorationSpeed;

    [DataMember(Name = "MountingHorizontalOutOfBreathMultiplier")]
    public float MountingHorizontalOutOfBreathMultiplier;

    [DataMember(Name = "MountingVerticalOutOfBreathMultiplier")]
    public float MountingVerticalOutOfBreathMultiplier;

    [DataMember(Name = "CompactHandling")]
    public bool CompactHandling;

    [DataMember(Name = "SightingRange")]
    public float SightingRange;

    [DataMember(Name = "AllowJam")]
    public bool AllowJam;

    [DataMember(Name = "AllowFeed")]
    public bool AllowFeed;

    [DataMember(Name = "AllowMisfire")]
    public bool AllowMisfire;

    [DataMember(Name = "AllowSlide")]
    public bool AllowSlide;

    [DataMember(Name = "BaseMalfunctionChance")]
    public float BaseMalfunctionChance;

    [DataMember(Name = "AimSensitivity")]
    [UnionMappings(JTokenType.Float, JTokenType.Array)]
    public Union<float, float[][]> AimSensitivity { get; set; } = 1f;

    [DataMember(Name = "DurabilityBurnRatio")]
    public float DurabilityBurnRatio = 1f;

    [DataMember(Name = "HeatFactorGun")]
    public float HeatFactorGun;

    [DataMember(Name = "CoolFactorGun")]
    public float CoolFactorGun;

    [DataMember(Name = "AllowOverheat")]
    public bool AllowOverheat;

    [DataMember(Name = "HeatFactorByShot")]
    public float HeatFactorByShot = 1f;

    [DataMember(Name = "CoolFactorGunMods")]
    public float CoolFactorGunMods = 1f;

    [DataMember(Name = "IsFlareGun")]
    public bool IsFlareGun;

    [DataMember(Name = "IsOneoff")]
    public bool IsOneoff;

    [DataMember(Name = "IsGrenadeLauncher")]
    public bool IsGrenadeLauncher;

    [DataMember(Name = "NoFiremodeOnBoltcatch")]
    public bool NoFiremodeOnBoltcatch;

    [DataMember(Name = "IsStationaryWeapon")]
    public bool IsStationaryWeapon;

    [DataMember(Name = "IsBeltMachineGun")]
    public bool IsBeltMachineGun;

    [DataMember(Name = "BlockLeftStance")]
    public bool BlockLeftStance;

    [DataMember(Name = "WithAnimatorAiming")]
    public bool WithAnimatorAiming;

    [DataMember(Name = "MinRepairDegradation")]
    public float MinRepairDegradation;

    [DataMember(Name = "MaxRepairDegradation")]
    public float MaxRepairDegradation;

    [DataMember(Name = "SizeReduceRight")]
    public int SizeReduceRight;

    [DataMember(Name = "FoldedSlot")]
    public string FoldedSlot;

    [DataMember(Name = "weapFireType")]
    public EFireMode[] weapFireType;

    [DataMember(Name = "BurstShotsCount")]
    public int BurstShotsCount = 3;
}