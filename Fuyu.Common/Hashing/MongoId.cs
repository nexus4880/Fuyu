using System;
using Fuyu.Common.Serialization;
using Newtonsoft.Json;

namespace Fuyu.Common.Hashing;

[Serializable]
[JsonConverter(typeof(MongoIdConverter))]
public readonly struct MongoId : IComparable<MongoId>, IEquatable<MongoId>, IParsable<MongoId>
{
    private static readonly Random _random = new Random();
    private readonly uint _timeStamp;
    private readonly ulong _counter;
    public static readonly int RequiredStringLength = 24;

    public static uint UnixTimestamp
    {
        get
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var currTime = DateTime.Now.ToUniversalTime() - startTime;
            var result = Convert.ToUInt32(Math.Abs(currTime.TotalSeconds));
            return result;
        }
    }

    public static ulong Hash
    {
        get
        {
            return (ulong)(((long)_random.Next(0, int.MaxValue) << 8) ^ (long)_random.Next(0, int.MaxValue));
        }
    }

    public MongoId(uint timeStamp, ulong counter)
    {
        _timeStamp = timeStamp;
        _counter = counter;
    }

    public static MongoId Parse(string str, IFormatProvider format)
    {
        if (str == null || str.Length != RequiredStringLength)
        {
            throw new Exception();
        }

        var timeStamp = GetTimestamp(str);
        var counter = GetCounter(str);

        return new MongoId(timeStamp, counter);
    }

    public static bool TryParse(string str, IFormatProvider format, out MongoId id)
    {
        if (str == null || str.Length != RequiredStringLength)
        {
            id = default;
            return false;
        }

        var timeStamp = GetTimestamp(str);
        var counter = GetCounter(str);

        id = new MongoId(timeStamp, counter);

        return true;
    }

    public static MongoId Generate()
    {
        var counter = Hash << 24;

        return new MongoId(UnixTimestamp, counter);
    }

    public MongoId(int accountId)
    {
        _timeStamp = UnixTimestamp;

        var num = Convert.ToUInt32(accountId);
        var num2 = Convert.ToUInt32(_random.Next(0, 16777215));

        _counter = 4294967296UL | (ulong)num;
        _counter <<= 24;
        _counter |= (ulong)num2;
    }

    // smethod_1
    public static uint GetTimestamp(string id)
    {
        return Convert.ToUInt32(id.Substring(0, 8), 16);
    }

    // smethod_2
    public static ulong GetCounter(string id)
    {
        return Convert.ToUInt64(id.Substring(8, 16), 16);
    }

    // method_0
    public MongoId Next()
    {
        return new MongoId(_timeStamp, _counter + 1);
    }

    public bool Equals(MongoId other)
    {
        return _timeStamp == other._timeStamp
            && _counter == other._counter;
    }

    public int CompareTo(MongoId other)
    {
        if (this == other)
        {
            return 0;
        }

        if (this > other)
        {
            return 1;
        }

        return -1;
    }

    public override string ToString()
    {
        return _timeStamp.ToString("X8").ToLower()
            + _counter.ToString("X16").ToLower();
    }

    public override bool Equals(object obj)
    {
        if (obj != null)
        {
            if (obj is MongoId)
            {
                var mongoID = (MongoId)obj;
                return mongoID == this;
            }

            string text;

            if ((text = obj as string) != null)
            {
                return text == this.ToString();
            }
        }

        return false;
    }

    public override int GetHashCode()
    {
        var num = Convert.ToUInt32(_counter >> 32) * 3637U;
        var num2 = Convert.ToUInt32(_counter << 32 >> 32) * 5807U;

        return (int)(_timeStamp ^ num ^ num2);
    }

    public static implicit operator string(MongoId mongoId)
    {
        return mongoId.ToString();
    }

    public static implicit operator MongoId(string id)
    {
        return Parse(id, null);
    }

    public static bool operator ==(string a, MongoId b)
    {
        return b.Equals((object)a);
    }

    public static bool operator !=(string a, MongoId b)
    {
        return !b.Equals((object)a);
    }

    public static bool operator ==(MongoId a, string b)
    {
        return a.Equals((object)b);
    }

    public static bool operator !=(MongoId a, string b)
    {
        return !a.Equals((object)b);
    }

    public static bool operator ==(MongoId a, MongoId b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(MongoId a, MongoId b)
    {
        return !a.Equals(b);
    }

    public static bool operator >(MongoId a, MongoId b)
    {
        return a._timeStamp > b._timeStamp
            || (a._timeStamp == b._timeStamp && a._counter > b._counter);
    }

    public static bool operator <(MongoId a, MongoId b)
    {
        return a._timeStamp < b._timeStamp
            || (a._timeStamp == b._timeStamp && a._counter < b._counter);
    }

    public static bool operator >=(MongoId a, MongoId b)
    {
        return a == b || a > b;
    }

    public static bool operator <=(MongoId a, MongoId b)
    {
        return a == b || a < b;
    }
}
