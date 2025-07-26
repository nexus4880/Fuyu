using System.Threading.Tasks;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Modding;

namespace Fuyu.Devtools.DisableAfkTimer;

public class Mod : AbstractMod
{
    public override string Id { get; } = "Fuyu.Devtool.DisableAfkTimer";

    public override string Name { get; } = "Fuyu-DisableAfkTimer";

    private readonly IGameDataRepository _gameData;

    public Mod(IGameDataRepository gameData)
    {
        _gameData = gameData;
    }

    public override async Task OnLoad()
    {
        var settings = await _gameData.GetSettingsAsync();

        // The client disables the timer if it's not positive
        settings["data"]["config"]["AFKTimeoutSeconds"] = -1;

        await _gameData.SetSettingsAsync(settings);
    }
}