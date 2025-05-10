using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles;

namespace Fuyu.Backend.BSG.Models.Raid;

[DataContract]
public class MatchLocalEndResult
{
    [DataMember(Name = "profile")]
    public Profile Profile { get; set; }

    [DataMember(Name = "result")]
    public EExitStatus Result { get; set; }

    [DataMember(Name = "killerId")]
    public string KillerId { get; set; }

    [DataMember(Name = "killerAid")]
    public string KillerAid { get; set; }

    [DataMember(Name = "exitName")]
    public string ExitName { get; set; }

    [DataMember(Name = "inSession")]
    public bool InSession { get; set; }

    [DataMember(Name = "favorite")]
    public bool Favorite { get; set; }

    [DataMember(Name = "playTime")]
    public int PlayTime { get; set; }
}