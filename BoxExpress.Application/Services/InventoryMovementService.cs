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

    public async Task<ApiResponse<bool>> AddAsync(Order order)
    {
        await _unitOfWork.BeginTransactionAsync();

        foreach (OrderItem orderItem in order.OrderItems)
        {
            //todo validar el inventario si tiene disponible 
            var inventory = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(inventoryMovementDTO.WarehouseId, inventoryMovementDTO.ProductVariantId);
            if (inventory == null)
            {
                throw new Exception("falla");
                // return ApiResponse<bool>.Fail($"No se encontró inventario en el almacén de origen para el producto variante {inventoryMovementDTO.ProductVariantId}.");
            }

            if ((inventory.Quantity + orderItem.Quantity) < 0)
            {
                throw new Exception("falla");
                // return ApiResponse<bool>.Fail($"Inventario insuficiente para el producto variante {inventoryMovementDTO.ProductVariantId}.");
            }

            inventory.UpdatedAt = DateTime.UtcNow;
            inventory.Quantity = orderItem.Quantity;
            await _unitOfWork.Inventories.UpdateAsync(inventory);
            await _unitOfWork.InventoryMovements.AddAsync(new InventoryMovement()
            {
                WarehouseId = order.WarehouseId.Value,
                MovementType = InventoryMovementType.OrderDelivered,
                OrderId = order.Id,
                ProductVariantId = orderItem.ProductVariantId,
                Quantity = orderItem.Quantity,
                Notes = "OrderDelivered",
                Reference = order.Id.ToString() + orderItem.ProductVariantId.ToString(),
            });
        }

        //InventoryMovementDTO inventoryMovementDTO

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true);
    }


}



// public async Task<ApiResponse<bool>> AddAsync(Order order)
// {
//     await _unitOfWork.BeginTransactionAsync();

//     try
//     {
//         // ... lógica de validación, actualización de inventario y registro de movimientos

//         await _unitOfWork.SaveChangesAsync();
//         await _unitOfWork.CommitAsync();

//         return ApiResponse<bool>.Success(true);
//     }
//     catch (Exception ex)
//     {
//         await _unitOfWork.RollbackAsync();
//         return ApiResponse<bool>.Fail($"Error: {ex.Message}");
//     }
// }
