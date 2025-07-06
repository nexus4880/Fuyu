using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemRepairKitComponent : IItemComponent
{
    [DataMember]
    public float Resource { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("MaxRepairResource"))
        {
            return null;
        }

        return new ItemRepairKitComponent
        {
            Resource = templateProperties.Value<float>("MaxRepairResource")
        };
    }
}