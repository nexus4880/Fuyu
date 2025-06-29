using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Modding;

#if NET9_0_OR_GREATER

public interface IConfigureServices
{
    static abstract void ConfigureServices(IServiceCollection services);
}

#else

public interface IConfigureServices
{
    /// Must manually be marked as static
    void ConfigureServices(IServiceCollection services);
}

#endif