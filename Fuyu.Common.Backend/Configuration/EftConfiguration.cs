using System.Runtime.Serialization;

namespace Fuyu.Common.Backend.Configuration;

[DataContract]
public class EftConfiguration : Config<EftConfiguration>
{
    public override string FileName => "eft.json";

    [DataMember]
    public string AccountsPath { get; set; } = "./Fuyu/Accounts/EFT/";

    [DataMember]
    public string ProfilesPath { get; set; } = "./Fuyu/Profiles/EFT/";

    [DataMember]
    public string ServerUrl { get; set; } = "https://localhost:44301";
}