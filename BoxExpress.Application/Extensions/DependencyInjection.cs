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
        services.AddScoped<IExcelExporter<WarehouseDto>, WarehouseExcelExporter>();
        services.AddScoped<IExcelExporter<WalletTransactionDto>, WalletTransactionsExporter>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<ITimeSlotService, TimeSlotService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddAutoMapper(typeof(AutoMapperProfile));
        return services;
    }
}
