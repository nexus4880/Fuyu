namespace Fuyu.Backend.BSG.Models.Raid;

public enum EExitStatus
{
    Survived,
    Killed,
    Left,
    Runner,
    MissingInAction,
    Transit
}

public static partial class Extensions
{
    public static bool ShouldLoseItems(this EExitStatus status) =>
        status is EExitStatus.Killed or EExitStatus.Left or EExitStatus.MissingInAction;

    public static bool ShouldItemsLoseFIR(this EExitStatus status) =>
        status is EExitStatus.Runner;
}