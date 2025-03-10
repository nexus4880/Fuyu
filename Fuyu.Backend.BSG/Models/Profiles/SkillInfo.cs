using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles.Skills;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class SkillInfo
{
    [DataMember]
    public Skill[] Common { get; set; }

    [DataMember]
    public Mastery[] Mastering { get; set; }
}