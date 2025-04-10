using BoxExpress.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IProductVariantRepository Variants { get; }
    IWarehouseInventoryRepository Inventories { get; }

    Task<int> SaveChangesAsync();
}