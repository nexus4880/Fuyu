using System.Runtime.Serialization;

namespace Fuyu.Common.Launcher.Models.Messages;

[DataContract]
public class SaveSettingsMessage : Message
{
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "data")]
    public SaveSettingsEntry[] Data { get; set; }
}