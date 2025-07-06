using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemLockableComponent : IItemComponent
{
    [DataMember]
    public bool Locked { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("isSecured")
            || !templateProperties.Value<bool>("isSecured"))
        {
            return null;
        }

        return new ItemLockableComponent { Locked = true };
    }
}