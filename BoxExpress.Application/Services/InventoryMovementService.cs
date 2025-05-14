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

    public InventoryMovementService(
        IInventoryMovementRepository inventoryMovementRepository,
        IMapper mapper,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> ProcessDeliveryAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

        foreach (OrderItem orderItem in order.OrderItems)
        {
            await AdjustInventoryAsync(new InventoryMovement()
            {
                WarehouseId = order.WarehouseId.Value,
                MovementType = InventoryMovementType.OrderDelivered,
                OrderId = order.Id,
                ProductVariantId = orderItem.ProductVariantId,
                Quantity = orderItem.Quantity * -1,
                Notes = "OrderDelivered",
                Reference = order.Id.ToString() + orderItem.ProductVariantId.ToString(),
            });
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> RevertDeliveryAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

        foreach (var orderItem in order.OrderItems)
        {
            var movement = new InventoryMovement
            {
                WarehouseId = order.WarehouseId.Value,
                ProductVariantId = orderItem.ProductVariantId,
                Quantity = orderItem.Quantity,
                MovementType = InventoryMovementType.OrderDeliveryReverted,
                OrderId = order.Id,
                Notes = "Reversión de entrega",
                Reference = $"Revert-Order-{order.Id}-{orderItem.ProductVariantId}"
            };

            await AdjustInventoryAsync(movement);
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true);
    }


    private async Task AdjustInventoryAsync(InventoryMovement movement)
    {
        var inventory = await _unitOfWork.Inventories.GetByWarehouseAndProductVariant(movement.WarehouseId, movement.ProductVariantId);
        if (inventory == null)
        {
            throw new Exception($"No se encontró inventario en la bodega {movement.WarehouseId} para el producto {movement.ProductVariantId}");
            // if (movement.Quantity < 0)
            //     throw new Exception($"No se encontró inventario en la bodega {movement.WarehouseId} para el producto {movement.ProductVariantId}");

            // inventory = new WarehouseInventory
            // {
            //     WarehouseId = movement.WarehouseId,
            //     ProductVariantId = movement.ProductVariantId,
            //     Quantity = 0,
            //     CreatedAt = DateTime.UtcNow
            // };
            // await _unitOfWork.Inventories.AddAsync(inventory);
        }

        if (inventory.Quantity + movement.Quantity < 0)
            throw new Exception($"Inventario insuficiente para el producto {movement.ProductVariantId} en la bodega {movement.WarehouseId}");

        inventory.Quantity += movement.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Inventories.UpdateAsync(inventory);

        movement.CreatedAt = DateTime.UtcNow; // si lo necesitas
        await _unitOfWork.InventoryMovements.AddAsync(movement);
    }
}