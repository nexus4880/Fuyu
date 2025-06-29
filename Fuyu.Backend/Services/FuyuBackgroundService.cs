using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.Configuration;
using Fuyu.Backend.Security;
using Fuyu.Common.Backend.Networking;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Fuyu servers...");

            var servers = _serviceProvider.GetServices<FuyuServer>().ToList();
            var certificate = _certificateService.GetOrCreateCertificate(
                _configuration.CertificatePath,
                _configuration.CertificatePassword);

            var builder = new WebHostBuilder();
            ConfigureKestrel(builder, servers, certificate);
            ConfigureApplication(builder, servers);

            var webHost = builder.Build();
            return webHost.RunAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error occurred while running Fuyu servers");
            throw;
        }
    }

    private static void ConfigureKestrel(WebHostBuilder builder, List<FuyuServer> servers, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
    {
        builder.UseKestrel(options =>
        {
            foreach (var server in servers)
            {
                options.ListenAnyIP(server.Port, listenOptions =>
                {
                    listenOptions.UseHttps(certificate);
                });
            }
        });
    }

    private static void ConfigureApplication(WebHostBuilder builder, List<FuyuServer> servers)
    {
        builder.Configure(app =>
        {
            app.UseWebSockets();
            app.Run(context =>
            {
                var requestPort = context.Connection.LocalPort;
                var server = servers.FirstOrDefault(s => s.Port == requestPort);

                if (server == null)
                {
                    throw new InvalidOperationException($"Received request on unhandled port: {requestPort}");
                }

                return server.OnRequestAsync(context);
            });
        });
    }
}