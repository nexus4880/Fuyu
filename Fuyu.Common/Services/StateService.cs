using System;
using Fuyu.Common.Collections;
using Fuyu.Common.IO;
using Fuyu.Common.Models.States;

namespace Fuyu.Common.Services;

public class StateService
{
    public static StateService Instance => _instance.Value;
    private static readonly Lazy<StateService> _instance = new(() => new StateService());

    private readonly ThreadList<State> _states;

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private StateService()
    {
        _states = new ThreadList<State>();
    }

    private int FindIndex(string id)
    {
        var states = _states.ToList();

        for (var i = states.Count - 1; i > 0; i--)
        {
            if (states[i].Id == id)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetOrAdd<T>(string id, T value)
    {
        var state = new State()
        {
            Id = id,
            Type = value.GetType(),
            Value = value
        };

        var index = FindIndex(id);

        if (index > -1)
        {
            _states.TrySet(index, state);
        }
        else
        {
            _states.Add(state);
        }

    }

    public T Get<T>(string id)
    {
        var index = FindIndex(id);

        if (index > -1)
        {
            _states.TryGet(index, out var state);

            if (state.Type == typeof(T))
            {
                return (T)state.Value;
            }
            else
            {
                var ex = new InvalidCastException(typeof(T).ToString());
                Terminal.WriteLine(ex);
                throw ex;
            }
        }
        else
        {
            var ex = new ArgumentException($"Could not find state {id}");
            Terminal.WriteLine(ex);
            throw ex;
        }
    }

    public void Remove(string id)
    {
        var index = FindIndex(id);

        if (index > -1)
        {
            _states.TryRemoveAt(index);
        }
        else
        {
            var ex = new ArgumentException($"Could not find state {id}");
            Terminal.WriteLine(ex);
            throw ex;
        }
    }
}