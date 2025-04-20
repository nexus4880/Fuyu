using System.Runtime.Serialization;
using Fuyu.Backend.BSG;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.BSG.Models.Items;

[DataContract]
public class ItemKeyComponent : IItemComponent
{
    [DataMember(Name = "NumberOfUsages")]
    public int NumberOfUsages { get; set; }

    public static object CreateComponent(JObject templateProperties)
    {
        if (!templateProperties.ContainsKey("MaximumNumberOfUsage"))
        {
            return null;
        }

        return new ItemKeyComponent
        {
            NumberOfUsages = 0
        };
    }
}