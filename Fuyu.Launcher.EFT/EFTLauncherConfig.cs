using System;
using System.IO;
using System.Runtime.Serialization;
using Fuyu.Common;

namespace Fuyu.Launcher.EFT;

[DataContract]
public class EFTLauncherConfig : Config<EFTLauncherConfig>
{
    public override string FileName { get; } = "fuyu.launcher.eft.json";

    public static EFTLauncherConfig Instance => _instance.Value;
    private static readonly Lazy<EFTLauncherConfig> _instance = new(() => new EFTLauncherConfig());

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private EFTLauncherConfig()
    {
        EFTAddress = "https://localhost:44301";
        GamePath = new DirectoryInfo(Environment.CurrentDirectory).FullName;
    }

    [DataMember(Name = "address")]
    public string EFTAddress { get; set; }

    [DataMember(Name = "gamepath")]
    public string GamePath { get; set; }
}