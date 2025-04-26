using System.Runtime.Serialization;

namespace Fuyu.Common.Launcher.Models.Settings;

public class TextSetting : Setting
{
    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "value")]
    public string Value { get; set; }

    public TextSetting()
    {
        Type = ESettingType.Text;
        AddOnSaveCallback(UpdateValue);
    }

    private void UpdateValue(string value)
    {
        Value = value;
    }
}