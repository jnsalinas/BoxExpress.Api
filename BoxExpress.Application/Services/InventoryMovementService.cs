using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;
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

    public async Task<ApiResponse<bool>> ProcessDeliveryAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

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

    public async Task<ApiResponse<bool>> RevertDeliveryAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

        await ApplyOrderInventoryAsync(
            order,
            fromHoldStatus: InventoryHoldStatus.Consumed,
            toHoldStatus: InventoryHoldStatus.Active,
            movementType: InventoryMovementType.OrderDeliveryReverted,
            quantityMultiplier: 1,
            movementNote: "Reversión de entrega",
            movementReferencePrefix: "Revert"
        );

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true);
    }

    public async Task AdjustInventoryAsync(InventoryMovement movement)
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
        inventory.ReservedQuantity += movement.Quantity;
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
        var inventoryHolds = await _inventoryHoldRepository.GetByOrderItemIdsAndStatus(orderItemIds, fromHoldStatus);
        var now = DateTime.UtcNow;

        foreach (var orderItem in order.OrderItems)
        {
            var hold = inventoryHolds.FirstOrDefault(h => h.OrderItemId == orderItem.Id);
            if (hold == null)
                throw new Exception($"No se encontró un hold con estado {fromHoldStatus} para el OrderItem {orderItem.Id}");

            hold.Status = toHoldStatus;
            hold.UpdatedAt = now;
            await _unitOfWork.InventoryHolds.UpdateAsync(hold);
            await AdjustInventoryAsync(new InventoryMovement
            {
                WarehouseId = order.WarehouseId.Value,
                ProductVariantId = orderItem.ProductVariantId,
                Quantity = orderItem.Quantity * quantityMultiplier,
                MovementType = movementType,
                OrderId = order.Id,
                Notes = movementNote,
                Reference = $"{movementReferencePrefix}-{order.Id}-{orderItem.ProductVariantId}",
                CreatedAt = now
            });
        }
    }
}