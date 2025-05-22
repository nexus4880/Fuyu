using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Responses;

[DataContract]
public class NotifierChannelCreateResponse
{
    // NOTE: unused in client
    //[DataMember(Name = "server")]
    //public string Server { get; set; }

    // NOTE: unused in client
    //[DataMember(Name = "channel_id")]
    //public string ChannelId { get; set; }

    [DataMember(Name = "url")]
    public string URL { get; set; }

    [DataMember(Name = "ws")]
    public string WS { get; set; }
}