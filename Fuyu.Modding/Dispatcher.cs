#if NET
using System;
using System.Collections.Generic;

namespace Fuyu.Modding;

public static class Dispatcher<TDelegate> where TDelegate : Delegate
{
    private static readonly HashSet<TDelegate> _delegates = [];

    public static void Register(TDelegate action)
    {
        _delegates.Add(action);
    }

    public static void Unregister(TDelegate action)
    {
        _delegates.Remove(action);
    }

    public static void Dispatch(params object[] args)
    {
        foreach (var action in _delegates)
        {
            action.DynamicInvoke(args);
        }
    }
}

#endif