using Microsoft.Extensions.DependencyInjection;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Exporters;
using BoxExpress.Application.Mappings;
using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Services;
using BoxExpress.Domain.Entities;

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
        services.AddScoped<IWarehouseInventoryTransferService, WarehouseInventoryTransferService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IExcelExporter<WarehouseDto>, WarehouseExcelExporter>();
        services.AddScoped<IExcelExporter<WalletTransactionDto>, WalletTransactionsExporter>();
        services.AddScoped<IExcelExporter<StoreDto>, StoreExcelExporter>();
        services.AddScoped<IExcelExporter<OrderDto>, OrderExporter>();
        services.AddScoped<IExcelExporter<WarehouseInventoryTransferDto>, WarehouseInventoryTransferExporter>();
        services.AddScoped<IExcelExporter<ProductDto>, WarehouseInventoryExporter>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<ITimeSlotService, TimeSlotService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWithdrawalRequestService, WithdrawalRequestService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        services.AddScoped<IInventoryMovementService, InventoryMovementService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IWarehouseInventoryService, WarehouseInventoryService>();
        services.AddScoped<IInventoryHoldService, InventoryHoldService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddAutoMapper(typeof(AutoMapperProfile));
        return services;
    }
}
