using System.Runtime.Serialization;
using Fuyu.Common.Launcher.Delegates;

namespace Fuyu.Common.Launcher.Models.Settings;

public class Setting
{
    [DataMember(Name = "type")]
    public ESettingType Type { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    public SaveCallback OnSave;
}