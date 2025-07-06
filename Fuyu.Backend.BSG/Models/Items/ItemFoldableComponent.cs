using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemFoldableComponent : IItemComponent
{
    [DataMember]
    public bool Folded { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("Foldable")
            || !templateProperties["Foldable"].Value<bool>())
        {
            return null;
        }

        return new ItemFoldableComponent();
    }
}