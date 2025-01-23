using System.Threading;

namespace Fuyu.Common.Collections;

public class ThreadObject<T>
{
    private T _object;
    private readonly Lock _lock;

    public ThreadObject(T value)
    {
        _object = value;
        _lock = new Lock();
    }

    public T Get()
    {
        lock (_lock)
        {
            return _object;
        }
    }

    public void Set(T value)
    {
        lock (_lock)
        {
            _object = value;
        }
    }
}