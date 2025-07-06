using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Profiles.Health;

[DataContract]
public class BodyPartInfo
{
    [DataMember]
    public BodyPart Head { get; set; }

    [DataMember]
    public BodyPart Chest { get; set; }

    [DataMember]
    public BodyPart Stomach { get; set; }

    [DataMember]
    public BodyPart LeftArm { get; set; }

    [DataMember]
    public BodyPart RightArm { get; set; }

    [DataMember]
    public BodyPart LeftLeg { get; set; }

    [DataMember]
    public BodyPart RightLeg { get; set; }

    // TODO: Refactor class to support IEnumerator?
    [IgnoreDataMember]
    public IEnumerable<BodyPart> AllBodyParts
    {
        get
        {
            yield return Head;
            yield return Chest;
            yield return Stomach;
            yield return LeftArm;
            yield return RightArm;
            yield return LeftLeg;
            yield return RightLeg;
        }
    }
}