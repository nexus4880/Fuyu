using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain.Services;

public class ProfileInitializationService
{
    private readonly ProfileService _profileService;
    private readonly ILogger<ProfileInitializationService> _logger;

    public ProfileInitializationService(
        ProfileService profileService,
        ILogger<ProfileInitializationService> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    public async Task InitializeAllProfilesAsync(EftProfile profile)
    {
        if (!profile.ShouldWipe && profile.Pmc?.Inventory is not null)
        {
            await _profileService.InitializeInventoryMatricesAsync(profile.Pmc);
            _logger.LogInformation("Initialized PMC inventory matrices: {Username} ({Id})",
                profile.Pmc.Info.Nickname, profile.Pmc._id);
        }
    }
}