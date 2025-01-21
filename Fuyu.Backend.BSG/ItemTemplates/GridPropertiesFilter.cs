using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class GridPropertiesFilter
{
    [DataMember(Name = "Filter")]
    public List<MongoId> Filter { get; set; }

    [DataMember(Name = "ExcludedFilter")]
    public List<MongoId> ExcludedFilter { get; set; }
}