using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemCultistAmuletComponent : IItemComponent
{
    [DataMember(Name = "NumberOfUsages")]
    public int NumberOfUsages { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("NumberOfUsages"))
        {
            return null;
        }

        return new ItemCultistAmuletComponent
        {
            NumberOfUsages = templateProperties.Value<int>("NumberOfUsages")
        };
    }
}