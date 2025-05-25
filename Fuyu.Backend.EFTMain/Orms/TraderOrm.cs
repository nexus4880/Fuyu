using System;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Databases;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.EFTMain.Orms;

public class TraderOrm
{
    public static TraderOrm Instance => instance.Value;
    private static readonly Lazy<TraderOrm> instance = new(() => new TraderOrm());

    private readonly TraderDatabase _traderDatabase;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private TraderOrm()
    {
        _traderDatabase = TraderDatabase.Instance;
    }

    public TraderTemplate GetTraderTemplate(MongoId traderId)
    {
        return _traderDatabase.GetTraderTemplates()[traderId];
    }

    public TraderAssort GetTraderAssort(MongoId traderId)
    {
        return _traderDatabase.GetTraderAssort(traderId);
    }
}