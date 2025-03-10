using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class ConditionCounter
{
    [DataMember]
    public MongoId id { get; set; }

    [DataMember]
    public int value { get; set; }

    [DataMember]
    public string sourceId { get; set; }

    [DataMember]
    public string type { get; set; }
}