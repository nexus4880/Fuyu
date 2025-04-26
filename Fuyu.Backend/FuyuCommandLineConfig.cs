using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend;

public class FuyuCommandLineConfig
{
    public static FuyuCommandLineConfig Instance { get; set; } = new FuyuCommandLineConfig();

    [ExportArgument]
    public string CertificatePath { get; set; }

    [ExportArgument]
    public string CertificatePassword { get; set; }

    public RootCommand CreateRootCommand(Func<Task> runHandler)
    {
        var certificatePathOption = new Option<string>(
            ["-c", "--certificate"],
            description: "Path to an existing SSL certificate or where to write a generated one (if null will only generate a certificate in memory)"
        );

        var certificatePasswordOption = new Option<string>(
            ["-p", "--certificate-password"],
            description: "Password to the SSL certificate"
        );

        var dumpLaunchArgumentsCommand = new Command(
            name: "dump",
            description: "Dumps the available arguments"
        );

        dumpLaunchArgumentsCommand.SetHandler(DumpLaunchArguments);

        var runCommand = new Command(
            name: "run",
            description: "Runs the application"
        );

        runCommand.SetHandler(runHandler);

        var rootCommand = new RootCommand("Fuyu Backend");

        rootCommand.AddCommand(dumpLaunchArgumentsCommand);
        rootCommand.AddCommand(runCommand);
        runCommand.AddOption(certificatePathOption);
        runCommand.AddOption(certificatePasswordOption);

        runCommand.SetHandler(async (string certificatePath, string certificatePassword) =>
        {
            CertificatePath = certificatePath;
            CertificatePassword = certificatePassword;
            await runHandler();
        }, certificatePathOption, certificatePasswordOption);

        return rootCommand;
    }

    internal void DumpLaunchArguments(InvocationContext context)
    {
        var properties =
            typeof(FuyuCommandLineConfig)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<ExportArgumentAttribute>() is not null)
            .ToList();

        var arguments = new List<object>(properties.Count);

        foreach (var property in properties)
        {
            arguments.Add(new
            {
                type = property.PropertyType.Name,
                name = property.Name
            });
        }

        Console.WriteLine(Json.Stringify(arguments));
        context.ExitCode = 1;
    }

    private class ExportArgumentAttribute : Attribute
    {
    }
}