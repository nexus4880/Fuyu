using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class GridProperties
{
    [DataMember(Name = "filters")]
    public List<GridPropertiesFilter> Filters { get; set; }

    [DataMember(Name = "cellsH")]
    public int CellsHorizontal { get; set; }

    [DataMember(Name = "cellsV")]
    public int CellsVertical { get; set; }

    [DataMember(Name = "minCount")]
    public int MinCount { get; set; }

    [DataMember(Name = "maxCount")]
    public int MaxCount { get; set; }

    [DataMember(Name = "maxWeight")]
    public int MaxWeight { get; set; }

    [DataMember(Name = "isSortingTable")]
    public bool IsSortingTable { get; set; }
}
