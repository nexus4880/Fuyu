using Newtonsoft.Json;

namespace Fuyu.Common.Serialization;

public static class Json
{
    public static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new UnionContractResolver(),
        Formatting = Formatting.None
    };

    public static T Parse<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
    }

    public static string Stringify(object o)
    {
        return JsonConvert.SerializeObject(o, jsonSerializerSettings);
    }

    public static T Clone<T>(object o)
    {
        var json = Stringify(o);
        return Parse<T>(json);
    }
}