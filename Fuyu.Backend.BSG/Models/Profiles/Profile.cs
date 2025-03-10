using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;
using Fuyu.Common.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Profiles;

// Assembly-CSharp.dll: EFT.Profile
[DataContract]
public class Profile
{
    [DataMember]
    public MongoId _id { get; set; }

    [DataMember]
    public int aid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public MongoId? savage { get; set; }

    [DataMember(Name = "karmaValue")]
    public float KarmaValue { get; set; }

    [DataMember]
    public ProfileInfo Info { get; set; }

    [DataMember]
    public CustomizationInfo Customization { get; set; }

    [DataMember]
    public Dictionary<MongoId, bool> Encyclopedia { get; set; }

    [DataMember]
    public HealthInfo Health { get; set; }

    [DataMember]
    public InventoryInfo Inventory { get; set; }

    [DataMember]
    public List<InsuredItem> InsuredItems { get; set; }

    [DataMember]
    public SkillInfo Skills { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public NotesInfo Notes { get; set; }

    [DataMember]
    public Dictionary<string, ConditionCounter> TaskConditionCounters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<QuestInfo> Quests { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<MongoId, int> Achievements { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [UnionMappings(JTokenType.Object, JTokenType.Array)]
    public Union<Dictionary<MongoId, int>, MongoId[]> Prestige { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public UnlockedInfo UnlockedInfo { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public MoneyTransferLimitInfo moneyTransferLimitData { get; set; }

    [DataMember]
    public List<BonusInfo> Bonuses { get; set; }

    [DataMember]
    public HideoutInfo Hideout { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public RagfairInfo RagfairInfo { get; set; }

    [DataMember]
    [UnionMappings(JTokenType.Object, JTokenType.Array)]
    public Union<Dictionary<MongoId, EWishlistGroup>, object[]> WishList { get; set; }

    [DataMember]
    public StatsInfo Stats { get; set; }

    [DataMember]
    public Dictionary<MongoId, int> CheckedMagazines { get; set; } = [];

    [DataMember]
    public List<MongoId> CheckedChambers { get; set; } = [];

    [DataMember(EmitDefaultValue = false)]
    [UnionMappings(JTokenType.Object, JTokenType.Array)]
    public Union<Dictionary<MongoId, TraderInfo>, object[]>? TradersInfo { get; set; }

    // NOTE: Deserialization works but is deserialized as
    // an array because the profile has "WishList": [] by default
    // instead we should access it from this method which will
    // turn it into a Dictionary if it isn't one already
    // -- nexus4880, 2024-10-31
    public Dictionary<MongoId, EWishlistGroup> GetWishList()
    {
        if (!WishList.IsValue1)
        {
            WishList = new Dictionary<MongoId, EWishlistGroup>();
        }

        return WishList.Value1;
    }

    // NOTE: Write proper clone method later
    // -- nexus4880, 2024-11-1
    public Profile Clone()
    {
        return Json.Clone<Profile>(this);
    }
}