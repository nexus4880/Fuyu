using System.Runtime.Serialization;
using Fuyu.Common.Launcher.Delegates;

namespace Fuyu.Common.Launcher.Models.Settings;

public class Setting
{
    [DataMember(Name = "type")]
    public ESettingType Type;

    [DataMember(Name = "id")]
    public string Id;

    [DataMember(Name = "name")]
    public string Name;

    public SaveCallback OnSave;
}