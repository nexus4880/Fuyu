using System;
using System.Reflection;
using Fuyu.Backend;
using Microsoft.AspNetCore.Builder;

public static class Extensions
{
    public static void ConfigureKestrel(this Type type, WebApplication app)
    {
        if (typeof(IConfigureKestrel).IsAssignableFrom(type))
        {
            var methodName = "IConfigureKestrel";
            var flags = BindingFlags.Public | BindingFlags.Static;
            var methodInfo = type.GetMethod(methodName, flags);
            var method = GetDelegate(methodInfo);
            method?.Invoke(app);
        }
    }

    private static Action<WebApplication> GetDelegate(MethodInfo methodInfo)
    {
#if NET9_0_OR_GREATER
        return methodInfo.CreateDelegate<Action<WebApplication>>();
#else
        return Delegate.CreateDelegate(typeof(Action<WebApplication>), methodInfo) as Action<WebApplication>;
#endif
    }
}