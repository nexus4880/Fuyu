using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Modding;

public static class Extensions
{
    public static void ConfigureServices(this Type type, IServiceCollection services)
    {
        if (typeof(IConfigureServices).IsAssignableFrom(type))
        {
            var methodName = "ConfigureServices";
            var flags = BindingFlags.Public | BindingFlags.Static;
            var methodInfo = type.GetMethod(methodName, flags);
            var method = GetDelegate(methodInfo);
            method?.Invoke(services);
        }
    }

    private static Action<IServiceCollection> GetDelegate(MethodInfo methodInfo)
    {
#if NET9_0_OR_GREATER
        return methodInfo.CreateDelegate<Action<IServiceCollection>>();
#else
        return Delegate.CreateDelegate(typeof(Action<IServiceCollection>), methodInfo) as Action<IServiceCollection>;
#endif
    }
}