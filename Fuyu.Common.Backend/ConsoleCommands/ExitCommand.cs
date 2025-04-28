using System;
using System.Threading.Tasks;

namespace Fuyu.Common.Backend.ConsoleCommands;

[ConsoleCommand("exit", helpText: "Quits the application", aliases: ["q", "quit"])]
public class ExitCommand : IConsoleCommand
{
    public Task InvokeAsync(ArraySegment<string> args)
    {
        Environment.ExitCode = 1;
        return Task.CompletedTask;
    }
}
