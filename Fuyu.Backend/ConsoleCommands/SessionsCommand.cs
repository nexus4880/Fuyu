using System;
using System.Threading.Tasks;
using Fuyu.Backend.EFTMain;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Fuyu.DependencyInjection.Attributes;

namespace Fuyu.Common.Backend.ConsoleCommands;

[ConsoleCommand("sessions", helpText: "[add | remove | dump] <aid | sessionId>")]
public class SessionsCommand : IConsoleCommand
{
    private readonly EftOrm _eftOrm;

    [Injectable]
    public SessionsCommand(/*[Inject] EftOrm eftOrm*/)
    {
        _eftOrm = EftOrm.Instance;
    }

    public Task InvokeAsync(ArraySegment<string> args)
    {
        if (args.Count > 0)
        {
            switch (args[0])
            {
                case "add":
                {
                    return InvokeAdd(args.Slice(1));
                }
                case "remove":
                {
                    return InvokeRemove(args.Slice(1));
                }
                case "dump":
                {
                    return InvokeDump(args.Slice(1));
                }
                default:
                {
                    Terminal.WriteLine($"Unknown argument {args[0]}");
                    break;
                }
            }
        }
        else
        {
            Terminal.WriteLine("Missing action, see help for more details");
        }
        
        return Task.CompletedTask;
    }

    private Task InvokeAdd(ArraySegment<string> args)
    {
        Terminal.WriteLine("Not implemented");
        return Task.CompletedTask;
    }

    private Task InvokeRemove(ArraySegment<string> args)
    {
        Terminal.WriteLine("Not implemented");
        return Task.CompletedTask;
    }

    private Task InvokeDump(ArraySegment<string> args)
    {
        Terminal.WriteLine(Json.Stringify(_eftOrm.GetSessions()));
        return Task.CompletedTask;
    }
}