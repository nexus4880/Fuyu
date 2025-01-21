using System.Runtime.Serialization;

namespace Fuyu.Common.Launcher.Models.Messages;

[DataContract]
public class SaveSettingsEntry
{
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "value")]
    public string Value { get; set; }
}