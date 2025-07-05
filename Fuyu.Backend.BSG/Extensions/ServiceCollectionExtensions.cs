using Fuyu.Backend.BSG.Repositories;
using Fuyu.Backend.BSG.Repositories.Abstractions;
using Fuyu.Backend.BSG.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fuyu.Backend.BSG.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBSGServices(this IServiceCollection services)
    {
        services.AddSingleton<ItemService>();
        services.AddSingleton<ItemFactoryService>();
        services.AddSingleton<InventoryService>();
        services.AddSingleton<IItemTemplateRepository, JsonItemTemplateRepository>();

        return services;
    }
}