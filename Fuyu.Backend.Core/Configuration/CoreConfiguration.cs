using System.Runtime.Serialization;
using Fuyu.Common;

namespace Fuyu.Backend.Core.Configuration;

[DataContract]
public class CoreConfiguration : Config<CoreConfiguration>
{
    public override string FileName => "core.json";

    [DataMember]
    public string AccountsPath { get; set; } = "./Fuyu/Accounts/Core/";
}