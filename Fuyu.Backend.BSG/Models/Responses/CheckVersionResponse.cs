using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Responses;

[DataContract]
public class CheckVersionResponse
{
    [DataMember(Name = "isvalid")]
    public bool IsValid { get; set; }

    // NOTE: unused in client
    //[DataMember(Name = "latestVersion")]
    //public string LatestVersion { get; set; }
}