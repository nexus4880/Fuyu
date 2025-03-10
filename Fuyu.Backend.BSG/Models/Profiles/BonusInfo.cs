using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles.Bonusses;
using Fuyu.Backend.BSG.Models.Profiles.Skills;
using Fuyu.Common.Hashing;
using Newtonsoft.Json.Converters;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class BonusInfo
{
    [DataMember]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public EBonusType type { get; set; }

    [DataMember]
    public MongoId id { get; set; }

    [DataMember]
    public double value { get; set; }

    [DataMember]
    public bool visible { get; set; } = true;

    [DataMember]
    public bool passive { get; set; } = true;

    [DataMember(EmitDefaultValue = false)]
    public bool? production { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string icon { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<MongoId> filter { get; set; }

    [DataMember(Name = "skillType")]
    public string SkillType { get; set; }

    [DataMember(Name = "skillName")]
    public ESkillType? SkillName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public MongoId? templateId { get; set; }
}