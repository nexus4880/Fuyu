using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Fuyu.Backend.BSG;
using Fuyu.Backend.Core;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Loaders;
using Fuyu.Common.Backend.Networking;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection;
using Fuyu.Modding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Fuyu.Backend;

public class Program
{
    static X509Certificate2 GenerateSelfSigned(string password, out byte[] bytes)
    {
        using var rsa = RSA.Create();

        var req = new CertificateRequest("cn=Fuyu", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var cert = req.CreateSelfSigned(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30));
        bytes = cert.Export(X509ContentType.Pfx, password);

        return X509CertificateLoader.LoadPkcs12(bytes, password);
    }

    static X509Certificate2 GetCertificate(string certificatePath, string certificatePassword)
    {
        X509Certificate2 certificate;

        if (certificatePath != null && File.Exists(certificatePath))
        {
            certificate = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, certificatePassword);
            if (DateTime.UtcNow > certificate.NotAfter)
            {
                throw new Exception("Certificate expiring too soon");
            }

            Terminal.WriteLine($"Loaded certificate {certificate.SubjectName.Name}");

            return certificate;
        }

        certificate = GenerateSelfSigned(certificatePassword, out var bytes);
        if (certificatePath != null)
        {
            VFS.WriteBytes(certificatePath, bytes);
            Terminal.WriteLine($"Wrote certificate to {certificatePath}");
        }

        return certificate;
    }

    static Task RunServer(string certificatePath, string certificatePassword, List<FuyuServer> servers)
    {
        var builder = new WebHostBuilder();
        var certificate = GetCertificate(certificatePath, certificatePassword);

        builder.UseKestrel(options =>
        {
            for (var i = 0; i < servers.Count; i++)
            {
                options.ListenAnyIP(servers[i].Port, listenOptions =>
                {
                    listenOptions.UseHttps(certificate);
                });
            }
        });

        builder.Configure(app =>
        {
            app.UseWebSockets();
            app.Run(ctx =>
            {
                // This is how we determine if the request was made to the EFT backend or the Fuyu backend
                // -- nexus4880, 2025-4-24
                var requestPort = ctx.Connection.LocalPort;
                for (var i = 0; i < servers.Count; i++)
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

        return builder.Build().RunAsync();
    }

    static async Task<int> Main(string[] args)
    {
        var config = FuyuCommandLineConfig.Instance;
        var rootCommand = config.CreateRootCommand(Run);
        var exitCode = await rootCommand.InvokeAsync(args);

        return exitCode;
    }

    static async Task Run()
    {
        Resx.SetSource("fuyu-backend", typeof(Program).Assembly);
        var config = FuyuCommandLineConfig.Instance;
        var container = new DependencyContainer();

        Terminal.SetLogConfig("Fuyu.Backend", "Fuyu/Logs/Backend.log");

        LoadDatabase(container);
        LoadServers(container);
        await LoadMods(container);

        Terminal.WriteLine("Done!");
        Terminal.WriteLine("You can now run commands.");
        Terminal.WriteLine("Users can now connect.");

        await RunServer(
            config.CertificatePath,
            config.CertificatePassword,
            container.ResolveAll<FuyuServer>()
        );

        await ModManager.Instance.UnloadAll();
    }

    static void LoadDatabase(DependencyContainer container)
    {
        Terminal.WriteLine("Loading database...");

        CoreLoader.Instance.Load();

        EftLoader.Instance.OnResxSet += ItemFactoryLoader.Instance.Load;
        EftLoader.Instance.OnLoadTraders += TraderLoader.Instance.Load;
        EftLoader.Instance.OnResxSet += SurveyLoader.Instance.Load;

        EftLoader.Instance.Load();
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
