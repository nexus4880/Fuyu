using System.Threading.Tasks;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.EFTMain.Services;

public class ProfileStartupService
{
    private readonly IProfileRepository _profileRepository;
    private readonly ProfileInitializationService _profileInitializationService;
    private readonly ILogger<ProfileStartupService> _logger;

    public ProfileStartupService(
        IProfileRepository profileRepository,
        ProfileInitializationService profileInitializationService,
        ILogger<ProfileStartupService> logger)
    {
        _profileRepository = profileRepository;
        _profileInitializationService = profileInitializationService;
        _logger = logger;
    }

    public async Task InitializeAllProfilesAsync()
    {
        var profiles = await _profileRepository.GetAllAsync();

        foreach (var profile in profiles)
        {
            await _profileInitializationService.InitializeAllProfilesAsync(profile);
        }

        _logger.LogInformation("Profile initialization completed for {ProfileCount} profiles", profiles.Count);
    }
}