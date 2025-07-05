using Fuyu.Backend.EFTMain.Repositories;
using Fuyu.Backend.EFTMain.Repositories.Abstractions;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Backend.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Backend.EFTMain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEftServices(this IServiceCollection services)
    {
        // Configuration
        services.Configure<EftConfiguration>(config => config.Load());

        // Repositories
        services.AddSingleton<IAccountRepository, JsonAccountRepository>();
        services.AddSingleton<ISessionRepository, JsonSessionRepository>();
        services.AddSingleton<IProfileRepository, JsonProfileRepository>();
        services.AddSingleton<IGameDataRepository, JsonGameDataRepository>();

        services.AddSingleton<AccountService>();
        services.AddSingleton<ProfileService>();
        services.AddSingleton<BotService>();
        services.AddSingleton<HandbookService>();
        services.AddSingleton<RagfairService>();
        services.AddSingleton<LocationService>();

        return services;
    }
}