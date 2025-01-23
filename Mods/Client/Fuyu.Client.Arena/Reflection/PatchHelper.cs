using System;
using System.Reflection;

namespace Fuyu.Client.Arena.Reflection;

public static class PatchHelper
{
    public static readonly Type[] Types;
    public const BindingFlags PrivateFlags = BindingFlags.Instance
                                            | BindingFlags.NonPublic
                                            | BindingFlags.DeclaredOnly;

    static PatchHelper()
    {
        Types = typeof(ETransportProtocolType).Assembly.GetTypes();
    }
}