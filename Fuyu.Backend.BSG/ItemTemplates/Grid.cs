using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class Grid
{
    [DataMember(Name = "_name")]
    public string Name { get; set; }

    [DataMember(Name = "_id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "_parent")]
    public MongoId Parent { get; set; }

    [DataMember(Name = "_props")]
    public GridProperties Properties { get; set; }

    [DataMember(Name = "_proto")]
    public string Proto { get; set; }
}
