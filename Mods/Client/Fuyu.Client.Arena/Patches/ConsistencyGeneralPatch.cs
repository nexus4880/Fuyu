// Disable general file validation, does NOT disable client bundles validation

using System.Reflection;
using System.Threading.Tasks;
using EFT;
using Fuyu.Client.Arena.Reflection;
using Fuyu.Common.Client.Reflection;

namespace Fuyu.Client.Arena.Patches;

public class ConsistencyGeneralPatch : AbstractPatch
{
    private static readonly MethodInfo _mi;

    static ConsistencyGeneralPatch()
    {
        var flags = PatchHelper.PrivateFlags;
        _mi = typeof(TarkovApplication).BaseType.GetMethod("RunFilesChecking", flags);
    }

    public ConsistencyGeneralPatch() : base("Fuyu.Client.Arena.ConsistencyGeneralPatch", EPatchType.Prefix)
    {
    }

    protected override MethodBase GetOriginalMethod()
    {
        return _mi;
    }

    protected static bool Patch(ref Task __result)
    {
        __result = Task.CompletedTask;

        // Don't run original code
        return false;
    }
}