using System;
using System.Linq;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Profiles.Health;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class HealthInfo
{
    [DataMember]
    public BodyPartInfo BodyParts { get; set; }

    [DataMember]
    public ClampedHealthStat<float> Energy { get; set; }

    [DataMember]
    public ClampedHealthStat<float> Hydration { get; set; }

    [DataMember]
    public ClampedHealthStat<float> Temperature { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ClampedHealthStat<float> Poison { get; set; }

    [DataMember]
    public int? UpdateTime { get; set; }

    public BodyPart GetBodyPart(EBodyPart bodyPart)
    {
        return bodyPart switch
        {
            EBodyPart.Head => BodyParts.Head,
            EBodyPart.Chest => BodyParts.Chest,
            EBodyPart.Stomach => BodyParts.Stomach,
            EBodyPart.LeftArm => BodyParts.LeftArm,
            EBodyPart.RightArm => BodyParts.RightArm,
            EBodyPart.LeftLeg => BodyParts.LeftLeg,
            EBodyPart.RightLeg => BodyParts.RightLeg,
            EBodyPart.Common => throw new NotImplementedException("Only used in client"),
            _ => null,
        };
    }

    /// <summary>
    /// Returns whether any <see cref="BodyPart"/> contain a <see cref="BodyPartEffect"/>
    /// </summary>
    [IgnoreDataMember]
    public bool HasEffects
    {
        get
        {
            return BodyParts.AllBodyParts.Any(x => x.Effects.Count > 0);
        }
    }

    // SKIPPED: Immortal
    // Reason: only used on BSG's internal server
}