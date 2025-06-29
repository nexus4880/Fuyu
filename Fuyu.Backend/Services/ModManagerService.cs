using System;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Modding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.Services;

public class ModManagerService : IHostedService
{
    private readonly ILogger<ModManagerService> _logger;
    private readonly ModManager _modManager;
    private readonly IServiceProvider _serviceProvider;

    public ModManagerService(
        ILogger<ModManagerService> logger,
        ModManager modManager,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _modManager = modManager;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading mods...");
        await _modManager.Load(_serviceProvider);
        _logger.LogInformation("Mods loaded successfully");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unloading mods...");
        await _modManager.UnloadAll();
        _logger.LogInformation("Mods unloaded successfully");
    }
}