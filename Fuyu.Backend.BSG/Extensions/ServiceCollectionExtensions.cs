using Fuyu.Backend.BSG.Repositories;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Backend.BSG.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBSGServices(this IServiceCollection services)
    {
        services.AddSingleton<IItemTemplateRepository, JsonItemTemplateRepository>();
        services.AddSingleton<ITraderRepository, JsonTraderRepository>();
        services.AddSingleton<IGameDataRepository, JsonGameDataRepository>();

        services.AddSingleton<ItemService>();
        services.AddSingleton<ItemFactoryService>();
        services.AddSingleton<InventoryService>();
        services.AddSingleton<HandbookService>();
        services.AddSingleton<RagfairService>();

        return services;
    }
}