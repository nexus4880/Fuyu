using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class SightsItemProperties : FunctionalModItemProperties
{
    [DataMember(Name = "ScopesCount")]
    public int ScopesCount;

    [DataMember(Name = "AimSensitivity")]
    public float[][] AimSensitivity;

    [DataMember(Name = "ModesCount")]
    public int[] ModesCount;

    [DataMember(Name = "Zooms")]
    public float[][] Zooms;

    [DataMember(Name = "CalibrationDistances")]
    public int[][] CalibrationDistances;

    [DataMember(Name = "CustomAimPlane")]
    public string CustomAimPlane;

    [DataMember(Name = "IsAdjustableOptic")]
    public bool IsAdjustableOptic;

    [DataMember(Name = "MinMaxFov")]
    public Vector3 MinMaxFov;

    [DataMember(Name = "ZoomSensitivity")]
    public float ZoomSensitivity;

    [DataMember(Name = "AdjustableOpticSensitivity")]
    public float AdjustableOpticSensitivity;

    [DataMember(Name = "AdjustableOpticSensitivityMax")]
    public float AdjustableOpticSensitivityMax;
}