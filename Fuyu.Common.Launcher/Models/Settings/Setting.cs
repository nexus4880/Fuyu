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

    private SaveCallback _onSave;

    public void AddOnSaveCallback(SaveCallback callback)
    {
        _onSave += callback;
    }

    public void RemoveOnSaveCallback(SaveCallback callback)
    {
        _onSave -= callback;
    }

    public void InvokeOnSave(string value)
    {
        _onSave?.Invoke(value);
    }
}