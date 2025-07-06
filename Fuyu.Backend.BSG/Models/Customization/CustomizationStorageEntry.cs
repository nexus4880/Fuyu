using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Customization;

[DataContract]
public class CustomizationStorageEntry
{
    [DataMember(Name = "id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "source")]
    public string Source { get; set; }

    [DataMember(Name = "type")]
    public string Type { get; set; }
}