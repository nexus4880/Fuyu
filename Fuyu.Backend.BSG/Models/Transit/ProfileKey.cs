using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class ProfileKey
{
    [DataMember(Name = "_id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "keyId")]
    public MongoId KeyId { get; set; }

    [DataMember(Name = "isSold")]
    public bool IsSold { get; set; }
}