using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemRepairableComponent : IItemComponent
{
    [DataMember]
    public float Durability { get; set; }

    [DataMember]
    public float MaxDurability { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("Durability")
            || !templateProperties.ContainsKey("MaxDurability"))
        {
            return null;
        }

        var durability = templateProperties.Value<float>("Durability");
        var maxDurability = templateProperties.Value<float>("MaxDurability");

        return new ItemRepairableComponent
        {
            Durability = durability,
            MaxDurability = maxDurability
        };
    }
}