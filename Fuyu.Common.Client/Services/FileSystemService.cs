using System;
using System.Collections.Concurrent;
using System.IO;

namespace Fuyu.Common.Client.Services;

public class FileSystemService
{
    public static FileSystemService Instance => _instance.Value;
    private static readonly Lazy<FileSystemService> _instance = new(() => new FileSystemService());

    private readonly ConcurrentDictionary<string, object> _writeLocks;

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private FileSystemService()
    {
        _writeLocks = new ConcurrentDictionary<string, object>();
    }

    public bool FileExists(string filepath)
    {
        return File.Exists(filepath);
    }

    public bool DirectoryExists(string filepath)
    {
        return Directory.Exists(filepath);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    // NOTE: we must prevent threads from accessing the same file at the
    //       same time. This way we can prevent data corruption when
    //       writing to the same file.
    public void WriteTextFile(string filepath, string text, bool append = false)
    {
        // create directory
        var path = Path.GetDirectoryName(filepath);

        if (!DirectoryExists(path))
        {
            CreateDirectory(path);
        }

        // get write mode
        var mode = append ? FileMode.Append : FileMode.Create;

        // get thread lock
        _writeLocks.TryAdd(filepath, new object());

        // write text
        lock (_writeLocks[filepath])
        {
            using var fs = new FileStream(filepath, mode, FileAccess.Write, FileShare.None);
            using var sw = new StreamWriter(fs);
            sw.Write(text);
        }

        // release thread lock
        _writeLocks.TryRemove(filepath, out _);
    }
}