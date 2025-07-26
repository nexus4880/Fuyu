using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.Configuration;
using Fuyu.Backend.Security;
using Fuyu.Common.Backend.Networking;
using Fuyu.Modding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fuyu.Backend.Services;

public class FuyuBackgroundService : BackgroundService
{
    private readonly ILogger<FuyuBackgroundService> _logger;
    private readonly FuyuConfiguration _configuration;
    private readonly ICertificateService _certificateService;
    private readonly IServiceProvider _serviceProvider;

    public delegate void ConfigureHttpServices(IServiceCollection services);
    public delegate void ConfigureHttpApplication(WebApplication app);

    public FuyuBackgroundService(
        ILogger<FuyuBackgroundService> logger,
        IOptions<FuyuConfiguration> configuration,
        ICertificateService certificateService,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration.Value;
        _certificateService = certificateService;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Fuyu servers...");

            var servers = _serviceProvider.GetServices<FuyuServer>().ToList();
            var certificate = _certificateService.GetOrCreateCertificate(
                _configuration.CertificatePath,
                _configuration.CertificatePassword);

            var builder = WebApplication.CreateBuilder();
            Dispatcher<ConfigureHttpServices>.Dispatch(builder.Services);
            builder.Logging.ClearProviders();
            builder.WebHost.ConfigureKestrel(options =>
            {
                foreach (var server in servers)
                {
                    options.ListenAnyIP(server.Port, listenOptions =>
                    {
                        if (server.IsHTTPS)
                        {
                            listenOptions.UseHttps(certificate);
                        }

                        _logger.LogInformation("Bound {ServerName} to {Port}", server.Name, server.Port);
                    });
                }
            });
            
            var app = builder.Build();
            Dispatcher<ConfigureHttpApplication>.Dispatch(app);
            
            app.UseWebSockets();
            app.MapGet("/nexus", async (ctx) =>
            {
                await ctx.Response.WriteAsync($"nexus says the current time is: {DateTime.UtcNow}");
            });
            app.MapFallback((context) =>
            {
                var requestPort = context.Connection.LocalPort;
                var server = servers.FirstOrDefault(s => s.Port == requestPort);

                if (server == null)
                {
                    throw new InvalidOperationException($"Received request on unhandled port: {requestPort}");
                }

                return server.OnRequestAsync(context);
            });

            await app.RunAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error occurred while running Fuyu servers");
            throw;
        }
    }
}