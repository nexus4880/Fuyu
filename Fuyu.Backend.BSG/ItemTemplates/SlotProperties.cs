using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class SlotProperties
{
    [DataMember(Name = "filters")]
    public List<SlotPropertiesFilter> Filters { get; set; }
}