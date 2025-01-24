using System;
using System.Reflection;
using Fuyu.Common.Client.Services;
using HarmonyLib;

namespace Fuyu.Common.Client.Reflection;

public abstract class AbstractPatch
{
    public readonly LogService LogService;

    public readonly string Id;
    public readonly EPatchType Type;
    public readonly Harmony Harmony;

    public AbstractPatch(string id, EPatchType type)
    {
        LogService = LogService.Instance;

        Id = id;
        Type = type;
        Harmony = new Harmony(Id);
    }

    protected abstract MethodBase GetOriginalMethod();

    private HarmonyMethod GetPatchMethod()
    {
        var mi = GetType().GetMethod("Patch", BindingFlags.NonPublic | BindingFlags.Static);
        return new HarmonyMethod(mi);
    }

    public void Enable()
    {
        LogService.WriteLine($"Enabling: {Id}");

        var patch = GetPatchMethod();
        var target = GetOriginalMethod();

        if (target == null)
        {
            var ex = new InvalidOperationException($"{Id}: GetOriginalMethod returns null");
            LogService.WriteLine(ex);
            throw ex;
        }

        switch (Type)
        {
            case EPatchType.Prefix:
                Harmony.Patch(target, prefix: patch);
                return;

            case EPatchType.Postfix:
                Harmony.Patch(target, postfix: patch);
                return;

            case EPatchType.Transpile:
                Harmony.Patch(target, transpiler: patch);
                return;

            default:
                throw new NotImplementedException("Patch type");
        }
    }

    public void Disable()
    {
        LogService.WriteLine($"Disabling: {Id}");
        Harmony.UnpatchSelf();
    }
}