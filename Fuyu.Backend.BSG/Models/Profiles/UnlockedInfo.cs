using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class UnlockedInfo
{
    [DataMember]
    public List<MongoId> unlockedProductionRecipe { get; set; }
}