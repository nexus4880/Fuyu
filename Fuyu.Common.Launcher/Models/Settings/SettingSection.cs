using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Common.Launcher.Models.Settings;

public class SettingSection
{
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "settings")]
    public List<Setting> Settings { get; set; }
}