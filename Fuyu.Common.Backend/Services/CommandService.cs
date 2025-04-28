using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fuyu.Common.Backend.ConsoleCommands;
using Fuyu.Common.IO;
using Fuyu.DependencyInjection;

namespace Fuyu.Common.Backend.Services;

public class CommandService
{
    private List<Type> _commands { get; } = [];

    private readonly DependencyContainer _container;

    public CommandService(DependencyContainer container)
    {
        _container = container;
    }

    public void RegisterCommand<T>() where T: IConsoleCommand
    {
        _commands.Add(typeof(T));
    }

    public IEnumerable<ConsoleCommandAttribute> GetAllConsoleCommandsAttributes()
    {
        foreach (var command in _commands)
        {
            var consoleCommand = command.GetCustomAttribute<ConsoleCommandAttribute>();
            if (consoleCommand != null)
            {
                yield return consoleCommand;
            }
        }
    }

    public IConsoleCommand GetConsoleCommand(string commandName)
    {
        foreach (var command in _commands)
        {
            var consoleCommand = command.GetCustomAttribute<ConsoleCommandAttribute>();
            if (consoleCommand != null)
            {
                string[] keywords = [consoleCommand.Command, .. consoleCommand.Aliases];
                if (keywords.Any(keyword => keyword.Equals(commandName, StringComparison.OrdinalIgnoreCase)))
                {
                    return _container.Resolve(command) as IConsoleCommand;
                }
            }
        }

        return null;
    }

    public Task ExecuteCommand(ArraySegment<string> args)
    {
        if (args.Count > 0)
        {
            var command = GetConsoleCommand(args[0]);
            if (command != null)
            {
                var commandArgs =
                    args.Count > 1 ?
                    args.Slice(1) :
                    ArraySegment<string>.Empty;

                return command.InvokeAsync(commandArgs);
            }

            Terminal.WriteLine($"Command {args[0]} not found");
        }

        return Task.CompletedTask;
    }
}