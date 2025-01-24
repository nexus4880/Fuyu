using System.Runtime.Serialization;
using Fuyu.Common.Launcher.Models.Messages;

namespace Fuyu.Launcher.Core.Models.Messages;

[DataContract]
public class ViewAccountGameMessage : Message
{
    [DataMember(Name = "game")]
    public string Game { get; set; }
}