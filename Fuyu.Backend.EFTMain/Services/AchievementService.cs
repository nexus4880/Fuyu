using System;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Services;

public class AchievementService
{
    public static AchievementService Instance => instance.Value;
    private static readonly Lazy<AchievementService> instance = new(() => new AchievementService());

    private readonly EftOrm _eftOrm;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private AchievementService()
    {
        _eftOrm = EftOrm.Instance;
    }

    // TODO: generate this
    // --seionmoya, 2024-01-09
    public AchievementStatisticResponse GetStatistics()
    {
        return _eftOrm.GetAchievementStatistics();
    }
}