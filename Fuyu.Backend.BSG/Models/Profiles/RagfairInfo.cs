using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Trading;

namespace Fuyu.Backend.BSG.Models.Profiles;

[DataContract]
public class RagfairInfo
{
    [DataMember]
    public float rating { get; set; }

    [DataMember]
    public bool isRatingGrowing { get; set; }

    [DataMember]
    public Offer[] offers { get; set; }
}