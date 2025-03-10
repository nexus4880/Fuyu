using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles.Quests;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class QuestInfo
{
    [DataMember]
    public string qid { get; set; }

    [DataMember]
    public long startTime { get; set; }

    [DataMember]
    public EQuestStatus status { get; set; }

    [DataMember]
    public Dictionary<EQuestStatus, long> statusTimers { get; set; }

    [DataMember(Name = "completedConditions")]
    public HashSet<MongoId> CompletedConditions { get; set; }

    [DataMember(Name = "availableAfter")]
    public long AvailableAfter { get; set; }
}