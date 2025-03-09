using System;
using System.IO;
using System.Runtime.Serialization;
using Fuyu.Common;

namespace Fuyu.Launcher.EFT;

[DataContract]
public class ModConfig : Config<ModConfig>
{
    public override string FileName { get; } = "fuyu.launcher.eft.json";

    public static ModConfig Instance => _instance.Value;
    private static readonly Lazy<ModConfig> _instance = new(() => new ModConfig());

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private ModConfig()
    {
        Address = "https://localhost:44301";
        GamePath = new DirectoryInfo(Environment.CurrentDirectory).FullName;
    }

    [DataMember(Name = "address")]
    public string Address { get; set; }

    [DataMember(Name = "gamepath")]
    public string GamePath { get; set; }
}