using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemMedKitComponent : IItemComponent
{
    [DataMember]
    public float HpResource { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("MaxHpResource"))
        {
            return null;
        }

        return new ItemMedKitComponent
        {
            HpResource = templateProperties.Value<float>("MaxHpResource")
        };
    }
}