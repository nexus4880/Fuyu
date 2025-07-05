using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Modding;

public interface IConfigureServices
{
#if NET7_0_OR_GREATER
    static abstract void ConfigureServices(IServiceCollection services);
#endif
}