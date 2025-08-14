using BoxExpress.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BoxExpress.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly BoxExpressDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;
    
    // Propiedad mejorada que verifica realmente el estado de la transacción
    public bool HasActiveTransaction => _transaction != null && 
                                      _transaction.GetDbTransaction()?.Connection != null &&
                                      _transaction.GetDbTransaction().Connection.State == System.Data.ConnectionState.Open;
    
    // Propiedades de repositorios
    public IProductRepository Products { get; }
    public IProductVariantRepository Variants { get; }
    public IWarehouseInventoryRepository Inventories { get; }
    public IWarehouseInventoryTransferRepository WarehouseInventoryTransfers { get; }
    public IWalletRepository Wallets { get; }
    public IStoreRepository Stores { get; }
    public IUserRepository Users { get; }
    public IInventoryMovementRepository InventoryMovements { get; }
    public IInventoryHoldRepository InventoryHolds { get; }
    public IOrderRepository Orders { get; }
    public IOrderItemRepository OrderItems { get; }
    public IOrderCategoryHistoryRepository OrderCategoryHistories { get; }
    public IOrderStatusHistoryRepository OrderStatusHistories { get; }
    public IWalletTransactionRepository WalletTransactions { get; }
    public IClientRepository Clients { get; }
    public IClientAddressRepository ClientAddresses { get; }
    public IWarehouseRepository Warehouses { get; }
    public IProductLoanRepository ProductLoans { get; }
    public IProductLoanDetailRepository ProductLoanDetails { get; }

    public UnitOfWork(
        BoxExpressDbContext context,
        ILogger<UnitOfWork> logger,
        IProductRepository products,
        IProductVariantRepository variants,
        IWarehouseInventoryRepository inventories,
        IWarehouseInventoryTransferRepository transfers,
        IInventoryMovementRepository movements,
        IWalletRepository wallets,
        IStoreRepository stores,
        IInventoryHoldRepository holds,
        IUserRepository users,
        IOrderRepository orders,
        IOrderItemRepository orderItems,
        IOrderCategoryHistoryRepository orderCategoryHistories,
        IOrderStatusHistoryRepository orderStatusHistories,
        IWalletTransactionRepository walletTransactions,
        IClientRepository clients,
        IClientAddressRepository clientAddresses,
        IWarehouseRepository warehouses,
        IProductLoanRepository productLoans,
        IProductLoanDetailRepository productLoanDetails)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Inicializar repositorios
        Products = products;
        Variants = variants;
        Inventories = inventories;
        InventoryHolds = holds;
        InventoryMovements = movements;
        WarehouseInventoryTransfers = transfers;
        Wallets = wallets;
        Stores = stores;
        Users = users;
        Orders = orders;
        OrderItems = orderItems;
        OrderCategoryHistories = orderCategoryHistories;
        OrderStatusHistories = orderStatusHistories;
        WalletTransactions = walletTransactions;
        Clients = clients;
        ClientAddresses = clientAddresses;
        Warehouses = warehouses;
        ProductLoans = productLoans;
        ProductLoanDetails = productLoanDetails;
    }

    /// <summary>
    /// Inicia una nueva transacción de base de datos
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        // Si ya hay una transacción activa, limpiarla primero
        if (HasActiveTransaction)
        {
            _logger?.LogWarning("Transacción activa detectada. Limpiando antes de iniciar nueva transacción.");
            await CleanupTransactionAsync();
        }

        // Limpiar el contexto antes de iniciar nueva transacción
        ClearContext();

        try
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            _logger?.LogDebug("Transacción iniciada exitosamente");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error al iniciar la transacción");
            _transaction = null;
            throw;
        }
    }

    /// <summary>
    /// Confirma la transacción activa
    /// </summary>
    public async Task CommitAsync()
    {
        if (!HasActiveTransaction)
        {
            _logger?.LogWarning("Se intentó confirmar una transacción que no existe");
            return;
        }

        try
        {
            await _transaction!.CommitAsync();
            _logger?.LogDebug("Transacción confirmada exitosamente");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error al confirmar la transacción");
            await RollbackAsync();
            throw;
        }
        finally
        {
            CleanupTransaction();
            ClearContext(); // ← LIMPIEZA AUTOMÁTICA DESPUÉS DEL COMMIT
        }
    }

    /// <summary>
    /// Revierte la transacción activa
    /// </summary>
    public async Task RollbackAsync()
    {
        if (!HasActiveTransaction)
        {
            _logger?.LogWarning("Se intentó revertir una transacción que no existe");
            return;
        }

        try
        {
            await _transaction!.RollbackAsync();
            _logger?.LogDebug("Transacción revertida exitosamente");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error al revertir la transacción: {Message}", ex.Message);
        }
        finally
        {
            CleanupTransaction();
            ClearContext(); // ← LIMPIEZA AUTOMÁTICA DESPUÉS DEL ROLLBACK
        }
    }

    /// <summary>
    /// Guarda los cambios en la base de datos
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        try
        {
            var result = await _context.SaveChangesAsync();
            _logger?.LogDebug("Cambios guardados exitosamente. Entradas afectadas: {Count}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error al guardar cambios en la base de datos");
            throw;
        }
    }

    /// <summary>
    /// Limpia la transacción y libera recursos
    /// </summary>
    private void CleanupTransaction()
    {
        if (_transaction != null)
        {
            try
            {
                _transaction.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al limpiar la transacción");
            }
            finally
            {
                _transaction = null;
            }
        }
    }

    /// <summary>
    /// Limpia la transacción de forma asíncrona
    /// </summary>
    private async Task CleanupTransactionAsync()
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            catch
            {
                // Ignorar errores de rollback durante limpieza
            }
            finally
            {
                CleanupTransaction();
            }
        }
    }

    /// <summary>
    /// Obtiene información del estado del contexto para debugging
    /// </summary>
    public string GetContextStatus()
    {
        try
        {
            var entries = _context.ChangeTracker.Entries();
            var added = entries.Count(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added);
            var modified = entries.Count(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Modified);
            var deleted = entries.Count(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted);
            var unchanged = entries.Count(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged);
            
            return $"Context Status - Added: {added}, Modified: {modified}, Deleted: {deleted}, Unchanged: {unchanged}, HasTransaction: {HasActiveTransaction}";
        }
        catch (Exception ex)
        {
            return $"Error getting context status: {ex.Message}";
        }
    }

    /// <summary>
    /// Limpia el contexto de Entity Framework
    /// </summary>
    public void ClearContext()
    {
        try
        {
            var changeCount = _context.ChangeTracker.Entries().Count();
            _context.ChangeTracker.Clear();
            _logger?.LogDebug("Contexto de Entity Framework limpiado. Entidades removidas: {Count}", changeCount);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error al limpiar el contexto de Entity Framework");
        }
    }

    /// <summary>
    /// Implementación del patrón Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Método protegido para implementar el patrón Dispose
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                if (HasActiveTransaction)
                {
                    _logger?.LogWarning("Dispose llamado con transacción activa. Haciendo rollback automático.");
                    CleanupTransactionAsync().Wait();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error durante el dispose del UnitOfWork");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}
