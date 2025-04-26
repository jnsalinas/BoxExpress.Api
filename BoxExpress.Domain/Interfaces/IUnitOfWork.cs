using System.Threading.Tasks;
using BoxExpress.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IProductVariantRepository Variants { get; }
    IWarehouseInventoryRepository Inventories { get; }
    IWalletRepository Wallets { get; }
    IStoreRepository Stores { get; }
    
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
