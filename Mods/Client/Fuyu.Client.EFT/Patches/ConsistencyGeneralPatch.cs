// Disable general file validation, does NOT disable client bundles validation

using System.Reflection;
using System.Threading.Tasks;
using EFT;
using Fuyu.Common.Client.Reflection;

namespace Fuyu.Client.EFT.Patches;

public class ConsistencyGeneralPatch : AbstractPatch
{
    public ConsistencyGeneralPatch() : base("Fuyu.Client.EFT.ConsistencyGeneralPatch", EPatchType.Prefix)
    {
    }

    protected override MethodBase GetOriginalMethod()
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        return typeof(TarkovApplication).BaseType.GetMethod("RunFilesChecking", flags);
    }

    protected static bool Patch(ref Task __result)
    {
        __result = Task.CompletedTask;

        // Don't run original code
        return false;
    }
}