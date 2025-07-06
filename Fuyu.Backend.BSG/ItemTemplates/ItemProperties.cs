using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Fuyu.Backend.BSG.Models.Common;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
[JsonPolymorphic]
public class ItemProperties
{
    [DataMember(Name = "Width")]
    public int Width { get; set; }

    [DataMember(Name = "AnimationVariantsNumber")]
    public int AnimationVariantsNumber { get; set; }

    [DataMember(Name = "BackgroundColor")]
    public ETaxonomyColor BackgroundColor { get; set; }

    [DataMember(Name = "CanRequireOnRagfair")]
    public bool CanRequireOnRagfair { get; set; }

    [DataMember(Name = "CanSellOnRagfair")]
    public bool CanSellOnRagfair { get; set; }

    [DataMember(Name = "ConflictingItems")]
    public MongoId[] ConflictingItems { get; set; } = [];

    [DataMember(Name = "CreditsPrice")]
    public int CreditsPrice { get; set; }

    [DataMember(Name = "Description")]
    public string Description { get; set; }

    [DataMember(Name = "DiscardLimit")]
    public int DiscardLimit { get; set; } = -1;

    [DataMember(Name = "DropSoundType")]
    public EItemDropSoundType DropSoundType { get; set; }

    [DataMember(Name = "ExaminedByDefault")]
    public bool ExaminedByDefault { get; set; }

    [DataMember(Name = "ExamineExperience")]
    public int ExamineExperience { get; set; }

    [DataMember(Name = "ExamineTime")]
    public float ExamineTime { get; set; }

    [DataMember(Name = "ExtraSizeDown")]
    public int ExtraSizeDown { get; set; }

    [DataMember(Name = "ExtraSizeForceAdd")]
    public bool ExtraSizeForceAdd { get; set; }

    [DataMember(Name = "ExtraSizeLeft")]
    public int ExtraSizeLeft { get; set; }

    [DataMember(Name = "ExtraSizeRight")]
    public int ExtraSizeRight { get; set; }

    [DataMember(Name = "ExtraSizeUp")]
    public int ExtraSizeUp { get; set; }

    [DataMember(Name = "Height")]
    public int Height { get; set; }

    [DataMember(Name = "HideEntrails")]
    public bool HideEntrails { get; set; }

    [DataMember(Name = "InsuranceDisabled")]
    public bool InsuranceDisabled { get; set; }

    [DataMember(Name = "IsAlwaysAvailableForInsurance")]
    public bool IsAlwaysAvailableForInsurance { get; set; }

    [DataMember(Name = "IsSpecialSlotOnly")]
    public bool IsSpecialSlotOnly { get; set; }

    [DataMember(Name = "IsUnremovable")]
    public bool IsUnremovable { get; set; }

    [DataMember(Name = "ItemSound")]
    public string ItemSound { get; set; }

    [DataMember(Name = "LootExperience")]
    public int LootExperience { get; set; }

    [DataMember(Name = "MergesWithChildren")]
    public bool MergesWithChildren { get; set; }

    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "NotShownInSlot")]
    public bool NotShownInSlot { get; set; }

    [DataMember(Name = "Prefab")]
    public ResourceKey Prefab { get; set; }

    [DataMember(Name = "QuestItem")]
    public bool QuestItem { get; set; }

    [DataMember(Name = "RagFairCommissionModifier")]
    public float RagFairCommissionModifier { get; set; } = 1f;

    [DataMember(Name = "RarityPvE")]
    public ELootRarity RarityPvE { get; set; }

    [DataMember(Name = "RepairCost")]
    public int RepairCost { get; set; }

    [DataMember(Name = "RepairSpeed")]
    public int RepairSpeed { get; set; }

    [DataMember(Name = "ShortName")]
    public string ShortName { get; set; }

    [DataMember(Name = "SpawnChance")]
    public float SpawnChance { get; set; }

    [DataMember(Name = "StackMaxSize")]
    public int StackMaxSize { get; set; }

    [DataMember(Name = "StackObjectsCount")]
    public int StackObjectsCount { get; set; }

    [DataMember(Name = "Unlootable")]
    public bool Unlootable { get; set; }

    [DataMember(Name = "UnlootableFromSide")]
    public EPlayerSideMask[] UnlootableFromSide { get; set; }

    [DataMember(Name = "UnlootableFromSlot")]
    public string UnlootableFromSlot { get; set; }

    [DataMember(Name = "UsePrefab")]
    public ResourceKey UsePrefab { get; set; }

    [DataMember(Name = "Weight")]
    public float Weight { get; set; }
}

public enum ETaxonomyColor
{
    blue,
    yellow,
    green,
    red,
    black,
    grey,
    violet,
    orange,
    tracerYellow,
    tracerGreen,
    tracerRed,
    @default
}

public enum EItemDropSoundType
{
    None,
    Pistol,
    SubMachineGun,
    Rifle
}

public enum ELootRarity
{
    Not_exist = -1,
    Common,
    Rare,
    Superrare
}

[Flags]
public enum EPlayerSideMask
{
    None = 0,
    Usec = 1,
    Bear = 2,
    Savage = 4,
    Pmc = 3,
    All = 7
}