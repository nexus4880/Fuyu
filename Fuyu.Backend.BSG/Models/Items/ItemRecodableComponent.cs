using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemRecodableComponent : IItemComponent
{
    [DataMember]
    public bool IsEncoded { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("IsEncoded"))
        {
            return null;
        }

        return new ItemRecodableComponent
        {
            IsEncoded = templateProperties.Value<bool>("IsEncoded")
        };
    }
}