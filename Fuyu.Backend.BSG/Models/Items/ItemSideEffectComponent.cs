using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemSideEffectComponent : IItemComponent
{
    [DataMember]
    public float Value { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("MaxResource"))
        {
            return null;
        }

        return new ItemSideEffectComponent
        {
            Value = templateProperties.Value<float>("MaxResource")
        };
    }
}