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

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();

        return services;
    }
}