using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemMapComponent : IItemComponent
{
    [DataMember]
    public List<MapMarker> Markers { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("MaxMarkersCount"))
        {
            return null;
        }

        var maxMarkersCount = templateProperties.Value<int>("MaxMarkersCount");

        return new ItemMapComponent
        {
            Markers = new List<MapMarker>(maxMarkersCount)
        };
    }
}