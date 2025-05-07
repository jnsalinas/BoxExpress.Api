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

    public WarehouseInventoryTransferService(
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IWarehouseInventoryTransferRepository repository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
        var transfer = await _repository.GetByIdWithDetailsAsync(transferId);
        if (transfer == null)
            return ApiResponse<bool>.Fail("La transferencia no existe.");

        var validationResult = await TryValidateTransferAsync(transferId);
        if (!validationResult.IsSuccess)
            return validationResult;

        // Ejecutar la transferencia
        foreach (var item in transfer.TransferDetails)
        {
            // Restar de origen
            var inventoryOrigin = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.FromWarehouseId, item.ProductVariantId);
            if (inventoryOrigin == null)
            {
                return ApiResponse<bool>.Fail($"No se encontró inventario en el almacén de origen para el producto variante {item.ProductVariantId}.");
            }
            inventoryOrigin.Quantity -= item.Quantity;
            await _unitOfWork.Inventories.UpdateAsync(inventoryOrigin);

            // Sumar en destino
            var inventoryDestination = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.ToWarehouseId, item.ProductVariantId);
            if (inventoryDestination == null)
            {
                inventoryDestination = new WarehouseInventory
                {
                    WarehouseId = transfer.ToWarehouseId,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity
                };
                await _unitOfWork.Inventories.AddAsync(inventoryDestination);
            }
            else
            {
                inventoryDestination.Quantity += item.Quantity;
                await _unitOfWork.Inventories.UpdateAsync(inventoryDestination);
            }
        }

        transfer.Status = InventoryTransferStatus.Accepted;
        transfer.AcceptedByUserId = userId;
        transfer.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.WarehouseInventoryTransfers.UpdateAsync(transfer);

        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.Success(true, null, "Transferencia aceptada correctamente.");
    }

    public async Task<ApiResponse<bool>> RejectTransferAsync(int transferId, int userId, string rejectionReason)
    {
        var transfer = await _repository.GetByIdWithDetailsAsync(transferId);
        if (transfer == null)
            return ApiResponse<bool>.Fail("La transferencia no existe.");

        transfer.Status = InventoryTransferStatus.Rejected;
        transfer.AcceptedByUserId = userId;
        transfer.UpdatedAt = DateTime.UtcNow;
        transfer.RejectionReason = rejectionReason;
        await _repository.UpdateAsync(transfer);
        return ApiResponse<bool>.Success(true, null, "Transferencia rechazada correctamente.");
    }

}
