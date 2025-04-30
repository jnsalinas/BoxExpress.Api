using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Infrastructure.Persistence;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Repositories;

namespace BoxExpress.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BoxExpressDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWarehouseInventoryRepository, WarehouseInventoryRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
        services.AddScoped<IOrderCategoryRepository, OrderCategoryRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
        services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
        services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
        services.AddScoped<IOrderCategoryHistoryRepository, OrderCategoryHistoryRepository>();
        services.AddScoped<IWarehouseInventoryTransferRepository, WarehouseInventoryTransferRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();


        return services;
    }
}