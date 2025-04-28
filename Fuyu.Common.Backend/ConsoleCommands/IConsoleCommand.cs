using System;
using System.Threading.Tasks;

namespace Fuyu.Common.Backend.ConsoleCommands;

public interface IConsoleCommand
{
    Task InvokeAsync(ArraySegment<string> args);
}
