using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG;
using Fuyu.Backend.Core;
using Fuyu.Backend.EFTMain.Loaders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend.Services;

public class DatabaseService : IHostedService
{
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading database...");

        //CoreLoader.Instance.Load();

        /*EftLoader.Instance.OnResxSet += ItemFactoryLoader.Instance.Load;
        EftLoader.Instance.OnLoadTraders += TraderLoader.Instance.Load;
        EftLoader.Instance.OnResxSet += SurveyLoader.Instance.Load;*/

        //EftLoader.Instance.Load();

        _logger.LogInformation("Database loaded successfully");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Database service stopped");
        return Task.CompletedTask;
    }
}