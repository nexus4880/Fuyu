using System;
using Fuyu.Common.Hashing;
using Newtonsoft.Json;

namespace Fuyu.Common.Serialization;

public class MongoIdConverter : JsonConverter<MongoId?>
{
    public override MongoId? ReadJson(JsonReader reader, Type objectType, MongoId? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value != null)
        {
            if (MongoId.TryParse((string)reader.Value, null, out MongoId mongoId))
            {
                return mongoId;
            }
        }

        return default;
    }

    public override void WriteJson(JsonWriter writer, MongoId? value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}
