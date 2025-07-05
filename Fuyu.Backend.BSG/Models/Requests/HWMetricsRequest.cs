using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Requests;

public class HWMetricsRequest
{
    [DataMember(Name = "output")]
    public string Output { get; set; }

    [DataMember(Name = "errors")]
    public string Errors { get; set; }
}