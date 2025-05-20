using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.ItemEvents;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemUpdatable
{
    // does not emit when 'null'
    [DataMember(Name = "FireMode", EmitDefaultValue = false)]
    public ItemFireModeComponent FireMode { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Repairable", EmitDefaultValue = false)]
    public ItemRepairableComponent Repairable { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Foldable", EmitDefaultValue = false)]
    public ItemFoldableComponent Foldable { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Sight", EmitDefaultValue = false)]
    public ItemSightComponent Sight { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Light", EmitDefaultValue = false)]
    public ItemLightComponent Light { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "MedKit", EmitDefaultValue = false)]
    public ItemMedKitComponent MedKit { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "CultistAmulet", EmitDefaultValue = false)]
    public ItemCultistAmuletComponent CultistAmulet { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Buff", EmitDefaultValue = false)]
    public ItemBuffComponent Buff { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Dogtag", EmitDefaultValue = false)]
    public ItemDogtagComponent Dogtag { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "FaceShield", EmitDefaultValue = false)]
    public ItemFaceShieldComponent FaceShield { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "FoodDrink", EmitDefaultValue = false)]
    public ItemFoodDrinkComponent FoodDrink { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Key", EmitDefaultValue = false)]
    public ItemKeyComponent Key { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Lockable", EmitDefaultValue = false)]
    public ItemLockableComponent Lockable { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Map", EmitDefaultValue = false)]
    public ItemMapComponent Map { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Recodable", EmitDefaultValue = false)]
    public ItemRecodableComponent Recodable { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "RepairKit", EmitDefaultValue = false)]
    public ItemRepairKitComponent RepairKit { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "SideEffect", EmitDefaultValue = false)]
    public ItemSideEffectComponent SideEffect { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "Tag", EmitDefaultValue = false)]
    public ItemTagComponent Tag { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "SpawnedInSession", EmitDefaultValue = false)]
    public bool? SpawnedInSession { get; set; }

    // does not emit when 'null'
    [DataMember(Name = "StackObjectsCount", EmitDefaultValue = false)]
    public int? StackObjectsCount { get; set; }

    [DataMember(Name = "PinLockState", EmitDefaultValue = false)]
    public EPinLockState PinLockState { get; set; }
}