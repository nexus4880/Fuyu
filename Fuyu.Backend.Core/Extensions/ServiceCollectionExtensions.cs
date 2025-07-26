using Fuyu.Backend.Core.Configuration;
using Fuyu.Backend.Core.Repositories;
using Fuyu.Backend.Core.Services;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Backend.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.Configure<CoreConfiguration>(config => config.Load());

        services.AddSingleton<ICoreAccountRepository, JsonCoreAccountRepository>();
        services.AddSingleton<ICoreSessionRepository, JsonCoreSessionRepository>();

        services.AddSingleton<AccountService>();

        return services;
    }
}