using System;

namespace Fuyu.Common.Backend.ConsoleCommands;

[AttributeUsage(AttributeTargets.Class)]
public class ConsoleCommandAttribute : Attribute
{
    public string Command { get; }
    public string HelpText { get; }
    public string[] Aliases { get; }

    public ConsoleCommandAttribute(string command, string helpText = null, params string[] aliases)
    {
        Command = command;
        HelpText = helpText ?? "No help text available";
        Aliases = [.. aliases];
    }
}
