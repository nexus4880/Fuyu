using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Fuyu.Common.IO;

public static class Terminal
{
    private static readonly Lock _lock = new Lock();
    private static string _filepath;

    static Terminal()
    {
        _filepath = "./Fuyu/Logs/trace.log";
    }

    public static void SetLogConfig(string filepath)
    {
        _filepath = filepath;
    }

    private static void WriteToFile(string text)
    {
        VFS.WriteTextFile(_filepath, text, true);
    }

    public static void WriteLine(string text, [CallerLineNumber] int callerLineNumber = default, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
    {
        var time = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
        var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
        // Assuming file name matches class name (it will more often than not)
        var line = $"[{time} - {fileName}.{callerMemberName}:{callerLineNumber}] {text}\n";

        lock (_lock)
        {
            Console.Write(line);
            WriteToFile(line);
        }
    }

    public static void WriteLine(object o, [CallerLineNumber] int callerLineNumber = default, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
    {
        if (o == null)
        {
            throw new NullReferenceException();
        }

        WriteLine(o.ToString(), callerLineNumber, callerFilePath, callerMemberName);
    }
}