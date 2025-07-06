using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class BipodItemProperties : FunctionalModItemProperties
{
    [DataMember(Name = "BipodRecoilMultiplier")]
    public float BipodRecoilMultiplier = 0.2f;

    [DataMember(Name = "BipodReturnHandSpeedMultiplier")]
    public float BipodReturnHandSpeedMultiplier = 1f;

    [DataMember(Name = "BipodCameraSnapMultiplier")]
    public float BipodCameraSnapMultiplier = 1f;

    [DataMember(Name = "BipodOutOfStaminaBreathMultiplier")]
    public float BipodOutOfStaminaBreathMultiplier = 0.1f;

    // NOTE: Only X and Y are used for these two, I just made it a
    // Vector3 because I'm too lazy to make a Vector2 type right now
    // -- nexus4880, 2024-10-18
    [DataMember(Name = "PitchLimitProneBipod")]
    public Vector3 PitchLimitProneBipod = new Vector3 { X = -10, Y = 10 };

    [DataMember(Name = "YawLimitProneBipod")]
    public Vector3 YawLimitProneBipod = new Vector3 { X = -10, Y = 10 };
}