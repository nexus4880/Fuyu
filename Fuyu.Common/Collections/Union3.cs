using System;

namespace Fuyu.Common.Collections;

/// <summary>
/// See note on <see cref="IUnion"/>
/// </summary>
public readonly struct Union<T1, T2, T3> : IUnion
{
    // NOTE: While we could just use object I have intentionally used
    // separate fields here in order to avoid boxing value types
    // -- nexus4880, 2024-10-22
    private readonly T1 _value1;
    private readonly T2 _value2;
    private readonly T3 _value3;
    private readonly byte _valueIndex;

    public T1 Value1
    {
        get
        {
            if (_valueIndex != 0)
            {
                throw new InvalidOperationException();
            }

            return _value1;
        }
    }

    public T2 Value2
    {
        get
        {
            if (_valueIndex != 1)
            {
                throw new InvalidOperationException();
            }

            return _value2;
        }
    }

    public T3 Value3
    {
        get
        {
            if (_valueIndex != 2)
            {
                throw new InvalidOperationException();
            }

            return _value3;
        }
    }

    object IUnion.Value
    {
        get
        {
            return _valueIndex switch
            {
                0 => _value1,
                1 => _value2,
                2 => _value3,
                _ => throw new InvalidOperationException(),
            };
        }
    }

    public Union(T1 value)
    {
        _value1 = value;
        _value2 = default;
        _value3 = default;
        _valueIndex = 0;
    }

    public Union(T2 value)
    {
        _value1 = default;
        _value3 = default;
        _value2 = value;
        _valueIndex = 1;
    }

    public Union(T3 value)
    {
        _value1 = default;
        _value2 = default;
        _value3 = value;
        _valueIndex = 2;
    }

    public static implicit operator Union<T1, T2, T3>(T1 value)
    {
        return new Union<T1, T2, T3>(value);
    }

    public static implicit operator Union<T1, T2, T3>(T2 value)
    {
        return new Union<T1, T2, T3>(value);
    }

    public static implicit operator Union<T1, T2, T3>(T3 value)
    {
        return new Union<T1, T2, T3>(value);
    }

    public static implicit operator T1(Union<T1, T2, T3> union)
    {
        return union._value1;
    }

    public static implicit operator T2(Union<T1, T2, T3> union)
    {
        return union._value2;
    }

    public static implicit operator T3(Union<T1, T2, T3> union)
    {
        return union._value3;
    }

    public void Match(Action<T1> callback1, Action<T2> callback2, Action<T3> callback3)
    {
        switch (_valueIndex)
        {
            case 0: callback1(_value1); break;
            case 1: callback2(_value2); break;
            case 2: callback3(_value3); break;
            default: throw new InvalidOperationException();
        }
    }

    public static bool operator ==(Union<T1, T2, T3> lhs, Union<T1, T2, T3> rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Union<T1, T2, T3> lhs, Union<T1, T2, T3> rhs)
    {
        return !lhs.Equals(rhs);
    }

    public override bool Equals(object obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return _valueIndex switch
        {
            0 => _value1.GetHashCode(),
            1 => _value1.GetHashCode(),
            2 => _value1.GetHashCode(),
            _ => throw new InvalidOperationException(),
        };
    }

    public override string ToString()
    {
        return _valueIndex switch
        {
            0 => _value1.ToString(),
            1 => _value1.ToString(),
            2 => _value1.ToString(),
            _ => throw new InvalidOperationException(),
        };
    }
}