using System.Numerics;
using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Profiles.Health;

/// <summary>
/// Class used to automatically clamp <see cref="Minimum"/> and <see cref="Maximum"/> values
/// </summary>
/// <typeparam name="T"></typeparam>
[DataContract]
public class ClampedHealthStat<T> where T : INumber<T>
{
    public ClampedHealthStat(T minimum, T maximum, T current, T overDamageReceivedMultiplier, T environmentDamageMultiplier)
    {
        Minimum = minimum;
        Maximum = maximum;
        Current = current;
        OverDamageReceivedMultiplier = overDamageReceivedMultiplier;
        EnvironmentDamageMultiplier = environmentDamageMultiplier;
    }

    [DataMember]
    public T Current
    {
        get
        {
            return _current;
        }
        set
        {
            _current = T.Clamp(value, Minimum, Maximum);
        }
    }

    private T _current;

    [DataMember]
    public T Minimum { get; set; }

    [DataMember]
    public T Maximum { get; set; }

    [DataMember]
    public T OverDamageReceivedMultiplier { get; set; }

    [DataMember]
    public T EnvironmentDamageMultiplier { get; set; }
}