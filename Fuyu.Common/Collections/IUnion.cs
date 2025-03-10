using System;
using Fuyu.Common.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fuyu.Common.Collections;

// NOTE: Any union member should also use the UnionMappingsAttribute in order to determine which
// type should be deserialized depending on the token.
// At the time of writing this it only supports different JTokenTypes.
/* Example:
public class UnionUsageExample
{
    // This is correct
    [UnionMappings(JTokenType.Integer, JTokenType.Float)]
    public Union<int, float> NumericValue { get; set; }

    // This is wrong
    [UnionMappings(JTokenType.Float, JTokenType.Float)]
    public Union<float, double> NumericValue2 { get; set; }
}
*/

[JsonConverter(typeof(UnionConverter))]
public interface IUnion
{
    object Value { get; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class UnionMappingsAttribute : Attribute
{
    public JTokenType[] Tokens { get; }

    public UnionMappingsAttribute(params JTokenType[] tokens)
    {
        Tokens = tokens;
    }
}