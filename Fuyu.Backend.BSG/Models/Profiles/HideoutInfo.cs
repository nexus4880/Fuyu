using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles.Hideout;
using Fuyu.Common.Collections;
using Fuyu.Common.Hashing;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class HideoutInfo
{
    // TODO: proper type
    [DataMember]
    public object Production { get; set; }

    // TODO: proper type
    [DataMember]
    public object Improvements { get; set; }

    [DataMember]
    public HideoutAreaInfo[] Areas { get; set; }

    [DataMember]
    public string Seed { get; set; }

    [DataMember(Name = "Customization")]
    public Dictionary<EHideoutCustomizationType, MongoId?> GlobalCustomization { get; set; }

    [DataMember]
    [UnionMappings(JTokenType.Object, JTokenType.Array)]
    public Union<Dictionary<string, MongoId>, object[]> MannequinPoses { get; set; }
}