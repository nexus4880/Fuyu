using System.Runtime.Serialization;

namespace Fuyu.Common.Launcher.Models.Settings;

public class Setting
{
    [DataMember(Name = "type")]
    public ESettingType Type;

    [DataMember(Name = "id")]
    public string Id;

    [DataMember(Name = "name")]
    public string Name;
}