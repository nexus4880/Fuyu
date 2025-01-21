using System;
using System.Collections.Generic;
using System.IO;

namespace Fuyu.Common.Launcher.Services;

public class ContentService
{
    public static ContentService Instance => _instance.Value;
    private static readonly Lazy<ContentService> _instance = new(() => new ContentService());

    //                          filepath      path    ret
    private readonly Dictionary<string, Func<string, Stream>> _loadCallbacks;

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private ContentService()
    {
        _loadCallbacks = [];
    }

    public void SetOrAddLoader(string path, Func<string, Stream> callback)
    {
        if (_loadCallbacks.ContainsKey(path))
        {
            _loadCallbacks[path] = callback;
        }
        else
        {
            _loadCallbacks.Add(path, callback);
        }
    }

    // return content as a Stream
    public Stream Load(string filepath)
    {
        foreach (var kvp in _loadCallbacks)
        {
            if (filepath == kvp.Key)
            {
                return kvp.Value(filepath);
            }
        }

        throw new ArgumentException("No content found on path");
    }
}