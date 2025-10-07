using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Services;

public class WarehouseInventoryTransferService : IWarehouseInventoryTransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseInventoryTransferRepository _repository;
    private readonly IMapper _mapper;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IInventoryHoldRepository _inventoryHoldRepository;
    private readonly IWarehouseInventoryTransferRepository _warehouseInventoryTransferRepository;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IInventoryHoldService _inventoryHoldService;
    private readonly IUserContext _userContext;
    private readonly IProductVariantRepository _productVariantRepository;
    public WarehouseInventoryTransferService(
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IInventoryHoldRepository inventoryHoldRepository,
        IWarehouseInventoryTransferRepository repository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IInventoryMovementService inventoryMovementService,
        IWarehouseInventoryTransferRepository warehouseInventoryTransferRepository,
        IInventoryHoldService inventoryHoldService,
        IUserContext userContext,
        IProductVariantRepository productVariantRepository)
    {
        _warehouseInventoryTransferRepository = warehouseInventoryTransferRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _inventoryHoldRepository = inventoryHoldRepository;
        _inventoryMovementService = inventoryMovementService;
        _inventoryHoldService = inventoryHoldService;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _productVariantRepository = productVariantRepository;
    }

    public async Task<ApiResponse<IEnumerable<WarehouseInventoryTransferDto>>> GetAllAsync(WarehouseInventoryTransferFilterDto filter)
    {
        var (transfers, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WarehouseInventoryTransferFilter>(filter));
        return ApiResponse<IEnumerable<WarehouseInventoryTransferDto>>.Success(_mapper.Map<List<WarehouseInventoryTransferDto>>(transfers), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<WarehouseInventoryTransferDto?>> GetByIdAsync(int id) =>
        ApiResponse<WarehouseInventoryTransferDto?>.Success(_mapper.Map<WarehouseInventoryTransferDto>(await _repository.GetByIdWithDetailsAsync(id)));

    public async Task<ApiResponse<bool>> TryValidateTransferAsync(int transferId)
    {
        var transfer = await _repository.GetByIdWithDetailsAsync(transferId);
        if (transfer == null)
            return ApiResponse<bool>.Fail("La transferencia no existe.");

        foreach (var item in transfer.TransferDetails)
        {
            var inventory = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.FromWarehouseId, item.ProductVariantId);

            if (inventory == null || inventory.Quantity < item.Quantity)
            {
                return ApiResponse<bool>.Fail($"Inventario insuficiente para el producto variante {item.ProductVariantId}.");
            }
        }

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> AcceptTransferAsync(int transferId, int userId)
    {
        await _unitOfWork.BeginTransactionAsync();
        var transfer = await _repository.GetByIdWithDetailsAsync(transferId);
        if (transfer == null)
            return ApiResponse<bool>.Fail("La transferencia no existe.");

        var validationResult = await TryValidateTransferAsync(transferId);
        if (!validationResult.IsSuccess)
            return validationResult;

        var inventoryHolds = await _inventoryHoldRepository.GetByTransferIdsAndStatus(transferId, InventoryHoldStatus.Active);
        // Ejecutar la transferencia
        foreach (var item in transfer.TransferDetails)
        {
            // Restar de origen
            var inventoryOrigin = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.FromWarehouseId, item.ProductVariantId);
            if (inventoryOrigin == null)
            {
                return ApiResponse<bool>.Fail($"No se encontró inventario en el almacén de origen para el producto variante {item.ProductVariantId}.");
            }


            // inventoryOrigin.UpdatedAt = DateTime.UtcNow;
            // inventoryOrigin.Quantity -= item.Quantity;
            // await _unitOfWork.Inventories.UpdateAsync(inventoryOrigin);

            // Sumar en destino
            var inventoryDestination = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.ToWarehouseId, item.ProductVariantId);
            if (inventoryDestination == null)
            {
                inventoryDestination = new WarehouseInventory
                {
                    WarehouseId = transfer.ToWarehouseId,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    StoreId = inventoryOrigin.StoreId,
                };
                await _unitOfWork.Inventories.AddAsync(inventoryDestination);
            }
            else
            {
                inventoryDestination.Quantity += item.Quantity;
                await _unitOfWork.Inventories.UpdateAsync(inventoryDestination);
            }

            #region Crear movimientos de inventario

            var hold = inventoryHolds.FirstOrDefault(h => h.WarehouseInventoryId == inventoryOrigin.Id);
            if (hold == null)
                throw new Exception($"No se encontró un hold para el WarehouseInventoryId {inventoryOrigin.Id}");

            hold.Status = InventoryHoldStatus.Consumed;
            hold.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.InventoryHolds.UpdateAsync(hold);

            await _inventoryMovementService.AdjustInventoryAsync(new InventoryMovement
            {
                WarehouseId = transfer.FromWarehouseId,
                MovementType = InventoryMovementType.TransferSent,
                TransferId = transfer.Id,
                CreatedAt = DateTime.UtcNow,
                Reference = transfer.Id.ToString(),
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity * -1,
                Notes = "Transferencia enviada",
                CreatorId = _userContext.UserId,
            });

            await _unitOfWork.InventoryMovements.AddAsync(new InventoryMovement
            {
                WarehouseId = transfer.ToWarehouseId,
                MovementType = InventoryMovementType.TransferReceived,
                TransferId = transfer.Id,
                CreatedAt = DateTime.UtcNow,
                Reference = transfer.Id.ToString(),
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                Notes = "Transferencia recibida",
                CreatorId = _userContext.UserId.Value,
            });
            #endregion
        }
        // Actualizar la transferencia
        transfer.Status = InventoryTransferStatus.Accepted;
        transfer.AcceptedByUserId = userId;
        transfer.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.WarehouseInventoryTransfers.UpdateAsync(transfer);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true, null, "Transferencia aceptada correctamente.");
    }

    public async Task<ApiResponse<bool>> RejectTransferAsync(int transferId, int userId, string rejectionReason)
    {
        var transfer = await _repository.GetByIdWithDetailsAsync(transferId);
        if (transfer == null)
            return ApiResponse<bool>.Fail("La transferencia no existe.");

        // todo liberar el inventario en hold
        var inventoryHolds = await _inventoryHoldRepository.GetByTransferIdsAndStatus(transferId, InventoryHoldStatus.Active);
        foreach (var hold in inventoryHolds)
        {
            hold.Status = InventoryHoldStatus.Released;
            hold.UpdatedAt = DateTime.UtcNow;
            await _inventoryHoldRepository.UpdateAsync(hold);
        }

        // liberar el inventario reservado
        var inventory = await _warehouseInventoryRepository.GetByWarehouseAndProductVariants(transfer.FromWarehouseId, transfer.TransferDetails.Select(x => x.ProductVariantId).ToList());
        foreach (var item in transfer.TransferDetails)
        {
            var inventoryItem = inventory.FirstOrDefault(x => x.ProductVariantId == item.ProductVariantId);
            if (inventoryItem != null)
            {
                inventoryItem.ReservedQuantity -= item.Quantity;
                await _warehouseInventoryRepository.UpdateAsync(inventoryItem);
            }
        }

        transfer.Status = InventoryTransferStatus.Rejected;
        transfer.AcceptedByUserId = userId;
        transfer.UpdatedAt = DateTime.UtcNow;
        transfer.RejectionReason = rejectionReason;
        await _repository.UpdateAsync(transfer);
        return ApiResponse<bool>.Success(true, null, "Transferencia rechazada correctamente.");
    }

    public async Task<ApiResponse<bool>> CreateTransferAsync(WarehouseInventoryTransferDto warehouseInventoryTransferDto)
    {
        var newTransfer = _mapper.Map<WarehouseInventoryTransfer>(warehouseInventoryTransferDto);
        var warehouseInventories = await _warehouseInventoryRepository
            .GetByWarehouseAndProductVariants(newTransfer.FromWarehouseId, newTransfer.TransferDetails.Select(x => x.ProductVariantId).ToList());

        if (warehouseInventories == null || !warehouseInventories.Any())
        {
            return ApiResponse<bool>.Fail("No se encontró inventario en la bodega de origen.");
        }

        await _unitOfWork.BeginTransactionAsync();
        // Crear la transferencia  
        newTransfer.CreatedAt = DateTime.UtcNow;
        newTransfer.Status = InventoryTransferStatus.Pending;
        newTransfer.CreatorId = _userContext.UserId.Value; //todo: poner usuario con la sesion
        await _unitOfWork.WarehouseInventoryTransfers.AddAsync(newTransfer);
        await _unitOfWork.SaveChangesAsync();

        //crear el hold de los productos
        foreach (var item in newTransfer.TransferDetails)
        {
            var holdResult = await _inventoryHoldService.CreateInventoryHoldAsync(
                    warehouseInventories.First(x => x.ProductVariantId == item.ProductVariantId),
                    item.Quantity,
                    InventoryHoldType.Transfer,
                    InventoryHoldStatus.Active,
                    null,
                    item.Id
            );
            if (!holdResult.IsSuccess)
                return ApiResponse<bool>.Fail(holdResult.Message ?? "Error al reservar el inventario");
        }

        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(newTransfer.Id > 0, null, "Inventario creado exitosamente");
    }

    public async Task<ApiResponse<int>> GetPendingTransfersAsync(WarehouseInventoryTransferFilterDto filter)
    {
        return ApiResponse<int>.Success(await _warehouseInventoryTransferRepository.GetPendingTransfersAsync(_mapper.Map<WarehouseInventoryTransferFilter>(filter)));
    }

    public async Task<ApiResponse<bool>> TransferStoreAsync(int warehouseId, List<WarehouseInventoryTransferStoreDto> dto)
    {
        //valida si el inventario esta en la bodega
        var inventories = await _warehouseInventoryRepository.GetByWarehouseIdAndProductVariantsIdAndStoresId(warehouseId, dto.Select(x => x.ProductVariantId).ToList(), dto.Select(x => x.StoreId).ToList());

        //valida si el item en esta en otra bodega para no permitir crear otro product variant 
        await _unitOfWork.BeginTransactionAsync();
        foreach (var item in dto)
        {
            var inventoryToDiscount = inventories.FirstOrDefault(x => x.ProductVariantId == item.ProductVariantId);
            if (item.StoreId == inventoryToDiscount.StoreId)
            {
                continue;
            }

            if (inventoryToDiscount == null)
            {
                return ApiResponse<bool>.Fail($"No se encontró inventario para el producto variante {item.ProductVariantId} en la bodega {warehouseId}");
            }

            var inventoryToAdd = inventories.FirstOrDefault(x => x.ProductVariant.ProductId == inventoryToDiscount.ProductVariant.ProductId && x.ProductVariant.Name == inventoryToDiscount.ProductVariant.Name && x.StoreId == item.StoreId);
            if (inventoryToAdd == null)
            {
                ProductVariant? newProductVariant = await _productVariantRepository.GetByProductNameVariantNameAndStoreId(inventoryToDiscount.ProductVariant.Product.Name, inventoryToDiscount.ProductVariant.Name, item.StoreId);
                if (newProductVariant == null)
                {
                    newProductVariant = new ProductVariant()
                    {
                        ProductId = inventoryToDiscount.ProductVariant.ProductId,
                        Name = inventoryToDiscount.ProductVariant.Name,
                        ShopifyVariantId = inventoryToDiscount.ProductVariant.ShopifyVariantId,
                        Sku = inventoryToDiscount.ProductVariant.Sku + "-" + item.StoreId,
                        Price = inventoryToDiscount.ProductVariant.Price,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };
                    await _unitOfWork.Variants.AddAsync(newProductVariant);
                }

                inventoryToAdd = new WarehouseInventory
                {
                    WarehouseId = warehouseId,
                    ProductVariant = newProductVariant,
                    StoreId = item.StoreId,
                    Quantity = item.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    ReservedQuantity = 0,
                    PendingReturnQuantity = 0,
                    DeliveredQuantity = 0,
                };

                await _unitOfWork.Inventories.AddAsync(inventoryToAdd);

                var newMovement = new InventoryMovement
                {
                    CreatorId = _userContext.UserId,
                    Notes = "Transferencia entre tiendas recibida",
                    CreatedAt = DateTime.UtcNow,
                    Reference = "Tienda origen: " + inventoryToDiscount.Store?.Name + " Tienda destino: " + inventoryToAdd.Store?.Name,
                    ProductVariant = newProductVariant,
                    Quantity = item.Quantity,
                    WarehouseId = warehouseId,
                    MovementType = InventoryMovementType.TransferStoreReceived,
                };
                await _unitOfWork.InventoryMovements.AddAsync(newMovement);
            }
            else
            {
                await _inventoryMovementService.AdjustInventoryAsync(new InventoryMovement
                {
                    ProductVariantId = inventoryToAdd.ProductVariantId,
                    Quantity = item.Quantity,
                    WarehouseId = warehouseId,
                    MovementType = InventoryMovementType.TransferStoreReceived,
                    CreatedAt = DateTime.UtcNow,
                    Reference = "Tienda origen: " + inventoryToDiscount.Store?.Name + " Tienda destino: " + inventoryToAdd.Store?.Name,
                    Notes = "Transferencia entre tiendas recibida",
                }, false, false);
            }

            await _inventoryMovementService.AdjustInventoryAsync(new InventoryMovement
            {
                WarehouseId = warehouseId,
                MovementType = InventoryMovementType.TransferStoreSent,
                CreatedAt = DateTime.UtcNow,
                Reference = "Tienda origen: " + inventoryToDiscount.Store?.Name + " Tienda destino: " + item.StoreId,
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity * -1,
                Notes = "Transferencia entre tiendas enviada",
                CreatorId = _userContext.UserId,
            }, false, false);
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true, null, "Inventario transferido exitosamente");
    }
}
