using System;

namespace Fuyu.Common.Client.Services;

public class LogService
{
    public static LogService Instance => _instance.Value;
    private static readonly Lazy<LogService> _instance = new(() => new LogService());

    private readonly FileSystemService _fileSystemService;

    private readonly object _lock;
    private string _prefix;
    private string _filepath;

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private LogService()
    {
        _fileSystemService = FileSystemService.Instance;

        _lock = new();
        _prefix = "Fuyu";
        _filepath = "./Fuyu/Logs/trace.log";
    }

    public void SetLogConfig(string prefix, string filepath)
    {
        _prefix = prefix;
        _filepath = filepath;
    }

    private void WriteToFile(string text)
    {
        _fileSystemService.WriteTextFile(_filepath, text, true);
    }

    public void WriteLine(string text)
    {
        var time = DateTime.UtcNow;
        var line = $"[{_prefix}][{time}]{text}\n";

        lock (_lock)
        {
            Console.Write(line);
            WriteToFile(line);
        }
    }

    public void WriteLine(object o)
    {
        if (o == null)
        {
            throw new NullReferenceException();
        }

        WriteLine(o.ToString());
    }

    public string ReadLine()
    {
        return Console.In.ReadLine();
    }
}