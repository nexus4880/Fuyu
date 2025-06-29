using System.Threading.Tasks;

namespace Fuyu.Modding;

public abstract class AbstractMod
{
    internal bool IsLoaded { get; set; }

    /// <summary>
    /// Your plugin will be automatically registered into this dependency container with this ID.
    /// </summary>
    public abstract string Id { get; }

    /// <summary>
    /// This is the name that will be used in logs.
    /// </summary>
    public abstract string Name { get; }

    // TODO: Load dependencies first
    public virtual string[] Dependencies { get; }

    /// <summary>
    /// Gets called after the server has set up everything.
    /// </summary>
    /// <param name="container"></param>
    public virtual Task OnLoad()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets called whenever the server shuts down
    /// </summary>
    public virtual Task OnShutdown()
    {
        return Task.CompletedTask;
    }
}