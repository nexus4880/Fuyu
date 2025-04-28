using System;
using System.Text;
using System.Threading.Tasks;
using Fuyu.Common.Backend.Services;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection.Attributes;

namespace Fuyu.Common.Backend.ConsoleCommands;

[ConsoleCommand("help", helpText: "Provides information on a specified command or lists all available commands")]
public class HelpCommand : IConsoleCommand
{
    private readonly CommandService _commandService;

    [Injectable]
    public HelpCommand([Inject] CommandService commandService)
    {
        _commandService = commandService;
    }

    public Task InvokeAsync(ArraySegment<string> args)
    {
        var builder = new StringBuilder();
        builder.AppendLine("The following commands are available:");
        foreach (var command in _commandService.GetAllConsoleCommandsAttributes())
        {
            builder.Append(command.Command);
            if (command.Aliases.Length > 0)
            {
                builder.Append($" [{string.Join(", ", command.Aliases)}]:");
            }
            else
            {
                builder.Append(':');
            }

            builder.AppendLine($" {command.HelpText}");
        }

        Terminal.WriteLine(builder.ToString());
        return Task.CompletedTask;
    }
}
