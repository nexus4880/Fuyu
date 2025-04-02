using System.Runtime.Serialization;

namespace Fuyu.Launcher.EFT.Models.Configs;

[DataContract]
public class EFTStartupConfig
{
    [DataMember(Name = "BackendUrl")]
    public string BackendUrl { get; set; }

    [DataMember(Name = "Version")]
    public string Version { get; set; }

    [DataMember(Name = "MatchingVersion")]
    public string MatchingVersion { get; set; }
}