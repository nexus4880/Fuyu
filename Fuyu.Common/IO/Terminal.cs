using System;
using System.Threading;

namespace Fuyu.Common.IO;

public static class Terminal
{
    private static readonly Lock _lock = new Lock();
    private static string _prefix;
    private static string _filepath;

    static Terminal()
    {
        _prefix = "Fuyu";
        _filepath = "./Fuyu/Logs/trace.log";
    }

    public static void SetLogConfig(string prefix, string filepath)
    {
        _prefix = prefix;
        _filepath = filepath;
    }

    private static void WriteToFile(string text)
    {
        VFS.WriteTextFile(_filepath, text, true);
    }

    public static void WriteLine(string text)
    {
        var time = DateTime.UtcNow;
        var line = $"[{_prefix}][{time}]{text}\n";

        lock (_lock)
        {
            Console.Write(line);
            WriteToFile(line);
        }
    }

    public static void WriteLine(object o)
    {
        if (o == null)
        {
            throw new NullReferenceException();
        }

        WriteLine(o.ToString());
    }

    public static string ReadLine()
    {
        return Console.In.ReadLine();
    }
}