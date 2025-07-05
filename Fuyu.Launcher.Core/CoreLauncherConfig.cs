using System;
using System.Runtime.Serialization;
using Fuyu.Common;

namespace Fuyu.Launcher.Core;

[DataContract]
public class CoreLauncherConfig : Config<CoreLauncherConfig>
{
    public static CoreLauncherConfig Instance => _instance.Value;
    private static readonly Lazy<CoreLauncherConfig> _instance = new(() => new CoreLauncherConfig());

    public override string FileName => "fuyu.launcher.core.json";

    private CoreLauncherConfig()
    {
        CoreAddress = "https://localhost:44300";
    }

    [DataMember(Name = "address")]
    public string CoreAddress { get; set; }
}