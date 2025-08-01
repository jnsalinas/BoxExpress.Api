using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services;

public class InventoryMovementService : IInventoryMovementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IMapper _mapper;
    private readonly IInventoryHoldRepository _inventoryHoldRepository;

    public InventoryMovementService(
        IInventoryMovementRepository inventoryMovementRepository,
        IMapper mapper,
        IInventoryHoldRepository inventoryHoldRepository,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _inventoryHoldRepository = inventoryHoldRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IEnumerable<InventoryMovementDto>>> GetAllAsync(InventoryMovementFilterDto filter)
    {
        var (transactions, totalCount) = await _inventoryMovementRepository.GetFilteredAsync(_mapper.Map<InventoryMovementFilter>(filter));
        return ApiResponse<IEnumerable<InventoryMovementDto>>.Success(_mapper.Map<List<InventoryMovementDto>>(transactions), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<bool>> ProcessDeliveryAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await ApplyOrderInventoryAsync(
                order,
                fromHoldStatus: InventoryHoldStatus.Active,
                toHoldStatus: InventoryHoldStatus.Consumed,
                movementType: InventoryMovementType.OrderDelivered,
                quantityMultiplier: -1,
                movementNote: "Orden entregada",
                movementReferencePrefix: "Delivered"
            );

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> RevertDeliveryAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

        await ApplyOrderInventoryAsync(
            order,
            fromHoldStatus: InventoryHoldStatus.Consumed,
            toHoldStatus: InventoryHoldStatus.Reverted,
            movementType: InventoryMovementType.OrderDeliveryReverted,
            quantityMultiplier: 1,
            movementNote: "Reversión de entrega",
            movementReferencePrefix: "Revert"
        );

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true);
    }

    public async Task AdjustInventoryAsync(InventoryMovement movement, bool moveReserved = true, bool movePendingReturn = false)
    {
        var inventory = await _unitOfWork.Inventories.GetByWarehouseAndProductVariant(movement.WarehouseId, movement.ProductVariantId);
        if (inventory == null)
        {
            throw new Exception($"No se encontró inventario en la bodega {movement.WarehouseId} para el producto {movement.ProductVariantId}");
        }

        if (inventory.Quantity + movement.Quantity < 0)
            throw new Exception($"Inventario insuficiente para el producto {movement.ProductVariantId} en la bodega {movement.WarehouseId}");

        inventory.Quantity += movement.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        if (moveReserved)
            inventory.ReservedQuantity += movement.Quantity;
        if (movePendingReturn)
            inventory.PendingReturnQuantity += movement.Quantity;

        await _unitOfWork.InventoryMovements.AddAsync(movement);
        await _unitOfWork.Inventories.UpdateAsync(inventory);
    }

    private async Task ApplyOrderInventoryAsync(
            Order order,
            InventoryHoldStatus fromHoldStatus,
            InventoryHoldStatus toHoldStatus,
            InventoryMovementType movementType,
            int quantityMultiplier,
            string movementNote,
            string movementReferencePrefix
        )
    {
        var orderItemIds = order.OrderItems.Select(x => x.Id).ToList();
        var inventoryHolds = await GetInventoryHoldsForOrderAsync(orderItemIds, fromHoldStatus, movementType);
        var now = DateTime.UtcNow;

        foreach (var orderItem in order.OrderItems)
        {
            var hold = inventoryHolds.FirstOrDefault(h => h.OrderItemId == orderItem.Id);
            if (hold == null)
                throw new Exception($"No se encontró un bloqueo de inventario con estado {fromHoldStatus} para el OrderItem {orderItem.Id}");

            await ProcessInventoryMovementAsync(order, orderItem, movementType, quantityMultiplier, movementNote, movementReferencePrefix, now, hold);

            await UpdateInventoryHoldStatusAsync(hold, toHoldStatus, now);
        }
    }

    private async Task<List<InventoryHold>> GetInventoryHoldsForOrderAsync(List<int> orderItemIds, InventoryHoldStatus fromHoldStatus, InventoryMovementType movementType)
    {
        // Obtener holds con el estado requerido
        var inventoryHolds = await _inventoryHoldRepository.GetByOrderItemIdsAndStatus(orderItemIds, fromHoldStatus);

        // Para entregas, incluir también holds pendientes de devolución
        if (movementType == InventoryMovementType.OrderDelivered)
        {
            var pendingReturnHold = await GetLatestPendingReturnHoldAsync(orderItemIds);
            if (pendingReturnHold != null)
                inventoryHolds.Add(pendingReturnHold);
        }

        return inventoryHolds;
    }

    private async Task<InventoryHold?> GetLatestPendingReturnHoldAsync(List<int> orderItemIds)
    {
        var pendingReturnHolds = await _inventoryHoldRepository.GetByOrderItemIdsAndStatus(orderItemIds, InventoryHoldStatus.PendingReturn);
        return pendingReturnHolds.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
    }

    private async Task ProcessInventoryMovementAsync(
        Order order,
        OrderItem orderItem,
        InventoryMovementType movementType,
        int quantityMultiplier,
        string movementNote,
        string movementReferencePrefix,
        DateTime now,
        InventoryHold hold)
    {
        // Crear el movimiento de inventario
        var movement = CreateInventoryMovement(order, orderItem, movementType, quantityMultiplier, movementNote, movementReferencePrefix, now);

        // Determinar qué cantidades ajustar
        var (moveReserved, movePendingReturn) = DetermineInventoryAdjustmentFlags(movementType, hold.Status);

        // Obtener inventario una sola vez para optimizar las operaciones
        var inventory = await GetWarehouseInventoryAsync(order.WarehouseId.Value, orderItem.ProductVariantId);

        // Procesar el ajuste de inventario
        await ProcessInventoryAdjustmentAsync(movement, moveReserved, movePendingReturn, inventory);

        // Actualizar QuantityDelivered solo para entregas
        if (movementType == InventoryMovementType.OrderDelivered)
        {
            await UpdateQuantityDeliveredAsync(inventory, orderItem.Quantity);
        }
    }

    private InventoryMovement CreateInventoryMovement(
        Order order,
        OrderItem orderItem,
        InventoryMovementType movementType,
        int quantityMultiplier,
        string movementNote,
        string movementReferencePrefix,
        DateTime now)
    {
        return new InventoryMovement
        {
            WarehouseId = order.WarehouseId.Value,
            ProductVariantId = orderItem.ProductVariantId,
            Quantity = orderItem.Quantity * quantityMultiplier,
            MovementType = movementType,
            OrderId = order.Id,
            Notes = movementNote,
            Reference = $"{movementReferencePrefix}-{order.Id}-{orderItem.ProductVariantId}",
            CreatedAt = now
        };
    }

    private (bool moveReserved, bool movePendingReturn) DetermineInventoryAdjustmentFlags(InventoryMovementType movementType, InventoryHoldStatus holdStatus)
    {
        // Para entregas, mover de reservado a consumido (excepto si ya está pendiente de devolución)
        var moveReserved = (movementType == InventoryMovementType.OrderDelivered) && holdStatus != InventoryHoldStatus.PendingReturn;
        
        // Para holds pendientes de devolución, ajustar esa cantidad
        var movePendingReturn = holdStatus == InventoryHoldStatus.PendingReturn;

        return (moveReserved, movePendingReturn);
    }

    private async Task<WarehouseInventory> GetWarehouseInventoryAsync(int warehouseId, int productVariantId)
    {
        var inventory = await _unitOfWork.Inventories.GetByWarehouseAndProductVariant(warehouseId, productVariantId);
        if (inventory == null)
        {
            throw new Exception($"No se encontró inventario en la bodega {warehouseId} para el producto {productVariantId}");
        }
        return inventory;
    }

    private async Task ProcessInventoryAdjustmentAsync(InventoryMovement movement, bool moveReserved, bool movePendingReturn, WarehouseInventory inventory)
    {
        // Validar que hay inventario suficiente
        if (inventory.Quantity + movement.Quantity < 0)
        {
            throw new Exception($"Inventario insuficiente para el producto {movement.ProductVariantId} en la bodega {movement.WarehouseId}");
        }

        // Actualizar cantidades del inventario
        inventory.Quantity += movement.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        // Ajustar cantidades reservadas si corresponde
        if (moveReserved)
            inventory.ReservedQuantity += movement.Quantity;
        
        // Ajustar cantidades pendientes de devolución si corresponde
        if (movePendingReturn)
            inventory.PendingReturnQuantity += movement.Quantity;

        // Guardar el movimiento y actualizar el inventario
        await _unitOfWork.InventoryMovements.AddAsync(movement);
        await _unitOfWork.Inventories.UpdateAsync(inventory);
    }

    private async Task UpdateQuantityDeliveredAsync(WarehouseInventory inventory, int quantity)
    {
        // Actualizar la cantidad entregada
        if(inventory.QuantityDelivered == null)
            inventory.QuantityDelivered = 0;
            
        inventory.QuantityDelivered += quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Inventories.UpdateAsync(inventory);
    }

    private async Task UpdateInventoryHoldStatusAsync(InventoryHold hold, InventoryHoldStatus newStatus, DateTime now)
    {
        hold.Status = newStatus;
        hold.UpdatedAt = now;
        await _unitOfWork.InventoryHolds.UpdateAsync(hold);
    }
}