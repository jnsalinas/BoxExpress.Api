using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;
using BoxExpress.Domain.Constants;

namespace BoxExpress.Application.Services;

public class InventoryHoldService : IInventoryHoldService
{
    private readonly IInventoryHoldRepository _repository;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IUserContext _userContext;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IFileService _fileService;

    public InventoryHoldService(IInventoryHoldRepository repository,
    IInventoryMovementService inventoryMovementService,
    IMapper mapper,
    IWarehouseInventoryRepository warehouseInventoryRepository,
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IOrderStatusHistoryRepository orderStatusHistoryRepository,
    IOrderStatusRepository orderStatusRepository,
    IFileService fileService)
    {
        _inventoryMovementService = inventoryMovementService;
        _repository = repository;
        _mapper = mapper;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _orderStatusRepository = orderStatusRepository;
        _fileService = fileService;
    }

    public async Task<ApiResponse<IEnumerable<InventoryHoldDto>>> GetAllAsync(InventoryHoldFilterDto filter)
    {
        filter.CountryId = _userContext?.CountryId != null ? _userContext.CountryId : filter.CountryId;
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

        int? orderStatusHistoryId = null;
        if (status == InventoryHoldStatus.PendingReturn)
        {
            var orderStatusHistory = await _orderStatusHistoryRepository.GetFilteredAsync(new OrderStatusHistoryFilter
            {
                OrderId = orderItems.First().OrderId,
                NewStatusId = _orderStatusRepository.GetByNameAsync(OrderStatusConstants.OnTheWay).Result?.Id ?? 0,
            });

            if (orderStatusHistory != null && orderStatusHistory.Any())
            {
                orderStatusHistoryId = orderStatusHistory.OrderByDescending(x => x.CreatedAt).First().Id;
            }
        }

        await _unitOfWork.BeginTransactionAsync();
        foreach (var item in orderItems)
        {
            if (warehouseInventories.Any(x => x.ProductVariantId == item.ProductVariantId))
            {
                var holdResult = await CreateInventoryHoldAsync(
                   warehouseInventory: warehouseInventories.First(x => x.ProductVariantId == item.ProductVariantId),
                   quantity: item.Quantity,
                   holdType: InventoryHoldType.Order,
                   holdStatus: status,
                   orderItemId: item.Id,
                   orderStatusHistoryId: orderStatusHistoryId
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
    int? productLoanDetailId = null,
    int? orderStatusHistoryId = null)
    {
        // Validación según el tipo de holdStatus
        if (holdStatus == InventoryHoldStatus.Active)
        {
            if (warehouseInventory.AvailableQuantity < quantity)
            {
                var variant = warehouseInventory.ProductVariant.Product.Name + " - " + warehouseInventory.ProductVariant?.Name ?? warehouseInventory.ProductVariantId.ToString();
                return ApiResponse<bool>.Fail($"Inventario insuficiente para el producto {variant}.");
            }

            warehouseInventory.ReservedQuantity += quantity;
        }
        else if (holdStatus == InventoryHoldStatus.PendingReturn)
        {
            if (warehouseInventory.OnTheWayQuantity < quantity)
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
                warehouseInventory.OnTheWayQuantity -= quantity;

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
            CreatorId = _userContext.UserId != null ? _userContext.UserId.Value : 1,
            ProductLoanDetailId = productLoanDetailId,
            OrderStatusHistoryId = orderStatusHistoryId
        });
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> AcceptReturnAsync(InventoryHoldResolutionDto dto)
    {
        InventoryHold? hold = await _repository.GetByIdWithDetailsAsync(dto.InventoryHoldId);
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
        if (dto.Photo != null)
            hold.OnRouteEvidenceUrl = await _fileService.UploadFileAsync(dto.Photo);

        var orderStatusHistoryCanceled = (await _orderStatusHistoryRepository.GetFilteredAsync(new OrderStatusHistoryFilter
        {
            OrderId = hold.OrderItem.OrderId,
            NewStatusId = _orderStatusRepository.GetByNameAsync(OrderStatusConstants.Cancelled).Result?.Id ?? 0,
        })).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

        if (orderStatusHistoryCanceled != null)
        {
            orderStatusHistoryCanceled.UpdatedAt = DateTime.UtcNow;
            orderStatusHistoryCanceled.Notes = !string.IsNullOrEmpty(orderStatusHistoryCanceled.Notes) ? $"{orderStatusHistoryCanceled.Notes} - {dto.Notes}" : dto.Notes;
            await _orderStatusHistoryRepository.UpdateAsync(orderStatusHistoryCanceled);
        }

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

    public async Task<ApiResponse<bool>> BulkAcceptReturnAsync(List<InventoryHoldResolutionDto> dto)
    {
        var itemResults = new List<ApiResponse<bool>>();
        foreach (var item in dto)
        {
            itemResults.Add(await AcceptReturnAsync(item));
        }
        return ApiResponse<bool>.Success(itemResults.Any(x => !x.IsSuccess));
    }

    public async Task<ApiResponse<bool>> BulkRejectReturnAsync(List<InventoryHoldResolutionDto> dto)
    {
        var itemResults = new List<ApiResponse<bool>>();
        foreach (var item in dto)
        {
            itemResults.Add(await RejectReturnAsync(item));
        }
        return ApiResponse<bool>.Success(itemResults.Any(x => !x.IsSuccess));
    }

    public async Task<ApiResponse<bool>> ReverseInventoryHoldAsync(int warehouseId, List<OrderItem> orderItems)
    {
        var warehouseInventories = await _warehouseInventoryRepository.GetByWarehouseAndProductVariants(warehouseId, orderItems.Select(x => x.ProductVariantId).ToList());
        if (warehouseInventories == null || !warehouseInventories.Any())
            return ApiResponse<bool>.Fail("No se encontró inventario en la bodega.");

        var holds = await _repository.GetByOrderItemIdsAndStatus(orderItems.Select(x => x.Id).ToList(), InventoryHoldStatus.Active);

        foreach (var orderItem in orderItems)
        {
            var warehouseInventory = warehouseInventories.First(x => x.ProductVariantId == orderItem.ProductVariantId);
            warehouseInventory.ReservedQuantity -= orderItem.Quantity;
            await _warehouseInventoryRepository.UpdateAsync(warehouseInventory);

            var hold = holds.First(x => x.OrderItemId == orderItem.Id);
            hold.Status = InventoryHoldStatus.Reverted;
            hold.Notes = !string.IsNullOrEmpty(hold.Notes) ? $"{hold.Notes} - Reversión de bloqueo" : "Reversión de bloqueo";
            hold.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(hold);
        }

        return ApiResponse<bool>.Success(true);
    }
}
