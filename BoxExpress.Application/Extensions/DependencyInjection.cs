using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using BoxExpress.Application.Mappings;
using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Services;

namespace BoxExpress.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IOrderStatusService, OrderStatusService>();
        services.AddScoped<IOrderCategoryService, OrderCategoryService>();
        services.AddScoped<IWalletTransactionService, WalletTransactionService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();

        services.AddAutoMapper(typeof(AutoMapperProfile));
        return services;
    }
}
