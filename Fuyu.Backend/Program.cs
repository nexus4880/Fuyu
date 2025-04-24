using System;
using System.Threading;
using System.Threading.Tasks;
using Fuyu.Backend.BSG;
using Fuyu.Backend.Core;
using Fuyu.Backend.EFTMain;
using Fuyu.Common.Backend.Networking;
using Fuyu.Common.Backend.Services;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Fuyu.DependencyInjection;
using Fuyu.Modding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Fuyu.Backend;

public class Program
{
    static Task RunServer(CancellationToken token, params FuyuServer[] servers)
    {
        var builder = new WebHostBuilder();
        builder.UseKestrel(options =>
        {
            for (var i = 0; i < servers.Length; i++)
            {
                options.ListenAnyIP(servers[i].Port, listenOptions =>
                {
                    // Add certificate stuff here?
                    listenOptions.UseHttps();
                });
            }
        });

        builder.Configure(app =>
        {
             app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(3d) });
             app.Run(ctx =>
             {
                 // This is how we determine if the request was made to the EFT backend or the Fuyu backend
                 // -- nexus4880, 2025-4-24
                 var requestPort = ctx.Connection.LocalPort;
                 for (var i = 0; i < servers.Length; i++)
                 {
                     var server = servers[i];
                     if (server.Port == requestPort)
                     {
                         return server.OnRequestAsync(ctx);
                     }
                 }

                 throw new Exception($"Received request on unhandled port: {requestPort} how?");
             });
        });

        return builder.Build().RunAsync(token);
    }

    static async Task Main(string[] args)
    {
        var container = new DependencyContainer();

        Terminal.SetLogConfig("Fuyu.Backend", "Fuyu/Logs/Backend.log");

        LoadDatabase(container);
        LoadServers(container);
        await LoadMods(container);

        Terminal.WriteLine("Done!");
        Terminal.WriteLine("You can now run commands.");
        Terminal.WriteLine("Users can now connect.");

        CommandService.Instance.OnSessions += _ =>
        {
            Terminal.WriteLine(Json.Stringify(EftOrm.Instance.GetSessions().Keys));
        };

        var cts = new CancellationTokenSource();
        var serverTask = RunServer(
            cts.Token,
            container.Resolve<FuyuServer, CoreServer>(),
            container.Resolve<FuyuServer, EftMainServer>()
        );

        while (CommandService.Instance.IsRunning)
        {
            var text = Terminal.ReadLine();
            if (text == null)
            {
                break;
            }

            var commandArgs = text.Split(' ');
            CommandService.Instance.RunCommand(commandArgs);
        }

        cts.Cancel();
        await serverTask;
        await ModManager.Instance.UnloadAll();
    }

    static void LoadDatabase(DependencyContainer container)
    {
        Terminal.WriteLine("Loading database...");

        CoreLoader.Instance.Load();
        EftLoader.Instance.OnResxSet += ItemFactoryLoader.Instance.Load;
        EftLoader.Instance.Load();
        TraderLoader.Instance.Load();
    }

    static void LoadServers(DependencyContainer container)
    {
        Terminal.WriteLine("Loading backends...");

        var coreServer = new CoreServer();
        container.RegisterSingleton<FuyuServer, CoreServer>(coreServer);

        coreServer.RegisterServices();

        var eftMainServer = new EftMainServer();
        container.RegisterSingleton<FuyuServer, EftMainServer>(eftMainServer);

        eftMainServer.RegisterServices();
    }

    static Task LoadMods(DependencyContainer container)
    {
        Terminal.WriteLine("Loading mods...");

        ModManager.Instance.AddMods("./Fuyu/Mods/Backend");
        return ModManager.Instance.Load(container);
    }
}