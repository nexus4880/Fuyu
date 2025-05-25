using System;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Orms;

namespace Fuyu.Backend.EFTMain.Services;

public class HideoutService
{
    public static HideoutService Instance => instance.Value;
    private static readonly Lazy<HideoutService> instance = new(() => new HideoutService());

    private readonly EftOrm _eftOrm;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private HideoutService()
    {
        _eftOrm = EftOrm.Instance;
    }

    public HideoutSettingsResponse GetSettings()
    {
        return _eftOrm.GetHideoutSettings();
    }
}