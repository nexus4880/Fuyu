using Fuyu.Backend.EFTMain.Factories;
using Fuyu.Backend.EFTMain.Factories.Abstractions;
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
        services.Configure<EftConfiguration>(config => config.Load());

        services.AddSingleton<IAccountRepository, JsonEftAccountRepository>();
        services.AddSingleton<ISessionRepository, JsonEftSessionRepository>();
        services.AddSingleton<IProfileRepository, JsonEftProfileRepository>();
        services.AddSingleton<IGameDataRepository, JsonGameDataRepository>();
        services.AddSingleton<ISurveyRepository, NullSurveyRepository>();

        services.AddSingleton<IProfileFactory, ProfileFactory>();

        services.AddScoped<AccountService>();
        services.AddScoped<ProfileService>();
        services.AddScoped<ProfileInitializationService>();
        services.AddScoped<ProfileStartupService>();
        services.AddScoped<BotService>();
        services.AddSingleton<HandbookService>();
        services.AddSingleton<RagfairService>();
        services.AddScoped<LocationService>();

        return services;
    }
}