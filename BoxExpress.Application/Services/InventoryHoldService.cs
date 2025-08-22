using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Services;

public class InventoryHoldService : IInventoryHoldService
{
    private readonly IInventoryHoldRepository _repository;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IUserContext _userContext;

    public InventoryHoldService(IInventoryHoldRepository repository,
    IInventoryMovementService inventoryMovementService,
    IMapper mapper,
    IWarehouseInventoryRepository warehouseInventoryRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext)
    {
        _inventoryMovementService = inventoryMovementService;
        _repository = repository;
        _mapper = mapper;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<ApiResponse<IEnumerable<InventoryHoldDto>>> GetAllAsync(InventoryHoldFilterDto filter)
    {
        var (inventoryHolds, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<InventoryHoldFilter>(filter));
        var mapped = _mapper.Map<List<InventoryHoldDto>>(inventoryHolds);

        var grouped = mapped.Where(x => x.OrderItemId.HasValue).GroupBy(x => x.OrderItemId);

        foreach (var group in grouped)
        {
            if (group.Count() > 1)
            {
                int index = 1;
                foreach (var item in group)
                {
                    item.ItemIndex = index++;
                }
            }
        }

        return ApiResponse<IEnumerable<InventoryHoldDto>>.Success(mapped, new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<bool>> HoldInventoryForOrderAsync(int warehouseId, List<OrderItem> orderItems, InventoryHoldStatus status)
    {
        var warehouseInventories = await _warehouseInventoryRepository
          .GetByWarehouseAndProductVariants(warehouseId, orderItems.Select(x => x.ProductVariantId).ToList());

        if (warehouseInventories == null || !warehouseInventories.Any())
            return ApiResponse<bool>.Fail("No se encontró inventario en la bodega.");

        await _unitOfWork.BeginTransactionAsync();
        foreach (var item in orderItems)
        {
            if (warehouseInventories.Any(x => x.ProductVariantId == item.ProductVariantId))
            {
                var holdResult = await CreateInventoryHoldAsync(
                   warehouseInventories.First(x => x.ProductVariantId == item.ProductVariantId),
                   item.Quantity,
                   InventoryHoldType.Order,
                   status,
                   item.Id
               );

                if (!holdResult.IsSuccess)
                    return ApiResponse<bool>.Fail(holdResult.Message ?? "Error al reservar el inventario");
            }
            else
            {
                return ApiResponse<bool>.Fail($"No se encontró inventario para la variante {item.ProductVariantId}.");
            }
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> CreateInventoryHoldAsync(
    WarehouseInventory warehouseInventory,
    int quantity,
    InventoryHoldType holdType,
    InventoryHoldStatus holdStatus,
    int? orderItemId = null,
    int? warehouseInventoryTransferDetailId = null, 
    int? productLoanDetailId = null)
    {
        // Validación según el tipo de holdStatus
        if (holdStatus == InventoryHoldStatus.Active)
        {
            if (warehouseInventory.AvailableQuantity < quantity)
            {
                var variant = warehouseInventory.ProductVariant?.Name ?? warehouseInventory.ProductVariantId.ToString();
                return ApiResponse<bool>.Fail($"Inventario insuficiente para el producto variante {variant}.");
            }

            warehouseInventory.ReservedQuantity += quantity;
        }
        else if (holdStatus == InventoryHoldStatus.PendingReturn)
        {
            if (warehouseInventory.ReservedQuantity < quantity)
            {
                var variant = warehouseInventory.ProductVariant?.Name ?? warehouseInventory.ProductVariantId.ToString();
                return ApiResponse<bool>.Fail($"No hay suficiente cantidad reservada para revertir la devolución del producto variante {variant}.");
            }

            var quantityToRelease = 0;
            var activeHolds = (await _repository.GetByWarehouseInventoryAndStatus(warehouseInventory.Id, InventoryHoldStatus.Active)).Where(x => x.Type == holdType);
            foreach (var hold in activeHolds.Where(h => h.OrderItemId == orderItemId))
            {
                hold.Status = InventoryHoldStatus.Released;
                quantityToRelease += hold.Quantity;
                await _unitOfWork.InventoryHolds.UpdateAsync(hold);
            }

            if (quantityToRelease < quantity)
            {
                var variant = warehouseInventory.ProductVariant?.Name ?? warehouseInventory.ProductVariantId.ToString();
                return ApiResponse<bool>.Fail($"No hay suficiente cantidad reservada para revertir la devolución del producto variante {variant}.");
            }

            if (quantityToRelease > 0)
                warehouseInventory.ReservedQuantity -= quantity;

            warehouseInventory.PendingReturnQuantity += quantity;
        }

        await _unitOfWork.Inventories.UpdateAsync(warehouseInventory);
        await _unitOfWork.InventoryHolds.AddAsync(new InventoryHold
        {
            WarehouseInventoryId = warehouseInventory.Id,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow,
            OrderItemId = orderItemId,
            WarehouseInventoryTransferDetailId = warehouseInventoryTransferDetailId,
            Type = holdType,
            Status = holdStatus,
            CreatorId = _userContext.UserId.Value,
            ProductLoanDetailId = productLoanDetailId
        });
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> AcceptReturnAsync(InventoryHoldResolutionDto dto)
    {
        InventoryHold? hold = await _repository.GetByIdAsync(dto.InventoryHoldId);
        if (hold == null || hold.Status != InventoryHoldStatus.PendingReturn)
            return ApiResponse<bool>.Fail("Devolución no encontrada o ya procesada.");

        var inventory = await _warehouseInventoryRepository.GetByIdAsync(hold.WarehouseInventoryId);
        if (inventory == null)
            return ApiResponse<bool>.Fail("Inventario no encontrado.");

        if (inventory.PendingReturnQuantity < hold.Quantity)
            return ApiResponse<bool>.Fail("Cantidad pendiente insuficiente.");

        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.PendingReturnQuantity -= hold.Quantity;
        hold.Status = InventoryHoldStatus.Returned;
        hold.Notes = dto.Notes;
        hold.UpdatedAt = DateTime.UtcNow;

        await _warehouseInventoryRepository.UpdateAsync(inventory);
        await _repository.UpdateAsync(hold);
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> RejectReturnAsync(InventoryHoldResolutionDto dto)
    {
        InventoryHold? hold = await _repository.GetByIdWithDetailsAsync(dto.InventoryHoldId);
        if (hold == null || hold.Status != InventoryHoldStatus.PendingReturn)
            return ApiResponse<bool>.Fail("Devolución no encontrada o ya procesada.");

        await _inventoryMovementService.AdjustInventoryAsync(new InventoryMovement()
        {
            CreatorId = _userContext.UserId.Value,
            WarehouseId = hold.WarehouseInventory.WarehouseId,
            ProductVariantId = hold.WarehouseInventory.ProductVariantId,
            Quantity = hold.Quantity * -1,
            MovementType = InventoryMovementType.LostOnCancellation,
            OrderId = hold.OrderItem.OrderId,
            Notes = dto.Notes,
            Reference = $"RejectReturn-{hold.OrderItem.OrderId}-{hold.WarehouseInventory.ProductVariantId}",
            CreatedAt = DateTime.UtcNow
        }, false, true);

        hold.UpdatedAt = DateTime.UtcNow;
        hold.Status = InventoryHoldStatus.NotReturned;
        hold.Notes = dto.Notes;
        await _repository.UpdateAsync(hold);
        return ApiResponse<bool>.Success(true);
    }
}
