using System.Threading.Tasks;
using Fuyu.Backend.BSG.Extensions;
using Fuyu.Backend.Configuration;
using Fuyu.Backend.Core.Extensions;
using Fuyu.Backend.EFTMain.Databases;
using Fuyu.Backend.EFTMain.Extensions;
using Fuyu.Backend.Logging;
using Fuyu.Backend.Services;
using Fuyu.Common.IO;
using Fuyu.Modding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fuyu.Backend;

public class Program
{
    public static async Task Main(string[] args)
    {
        InitializeApplication();

        var builder = Host.CreateApplicationBuilder(args);
        ConfigureServices(builder);

        var host = builder.Build();
        await host.RunAsync();
    }

    private static void InitializeApplication()
    {
        Resx.SetSource("fuyu-backend", typeof(Program).Assembly);
        Resx.SetSource("eft", typeof(EftDatabase).Assembly);
        Terminal.SetLogConfig("Fuyu/Logs/Backend.log");
    }

    private static void ConfigureServices(HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.FormatterName = "custom");
        builder.Logging.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>();

        builder.Services.Configure<FuyuConfiguration>(
            builder.Configuration.GetSection(FuyuConfiguration.SectionName));

        var loggerFactory = new LoggerFactory();
        var modManagerLogger = new Logger<ModManager>(loggerFactory);
        var modManager = new ModManager(modManagerLogger);
        builder.Services.AddSingleton(modManager);

        modManager.AddMods("./Fuyu/Mods/Backend", builder.Services);

        builder.Services.AddFuyuServices()
            .AddCoreServices()
            .AddBSGServices()
            .AddEftServices();
    }
}