using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseInventoryTransferRepository _warehouseInventoryTransferRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;

    public WarehouseService(
        IWarehouseInventoryTransferRepository warehouseInventoryTransferRepository,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IWarehouseRepository repository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _warehouseInventoryTransferRepository = warehouseInventoryTransferRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WarehouseDto>>> GetAllAsync(WarehouseFilterDto filter) =>
         ApiResponse<IEnumerable<WarehouseDto>>.Success(_mapper.Map<List<WarehouseDto>>(await _repository.GetFilteredAsync(_mapper.Map<WarehouseFilter>(filter))));

    public async Task<ApiResponse<WarehouseDetailDto?>> GetByIdAsync(int id) =>
        ApiResponse<WarehouseDetailDto?>.Success(_mapper.Map<WarehouseDetailDto>(await _repository.GetByIdWithDetailsAsync(id)));

    // UnitOfWork es un patrón que coordina varios repositorios para que puedas hacer múltiples operaciones sobre la base de datos en una única transacción.
    // ✅ Su objetivo es garantizar que todo se guarde o nada se guarde (atomicidad).
    public async Task<ApiResponse<bool>> AddInventoryToWarehouseAsync(int warehouseId, List<CreateProductWithVariantsDto> products)
    {
        try
        {
            foreach (var productDto in products)
            {
                Product product = new()
                {
                    Name = productDto.Name,
                    ShopifyProductId = productDto.ShopifyProductId,
                    StoreId = 1 // todo revisar si cada producto tiene una tienda asociada
                };

                await _unitOfWork.Products.AddAsync(product);

                foreach (var variantDto in productDto.Variants)
                {
                    ProductVariant variant = new()
                    {
                        Name = variantDto.Name,
                        ShopifyVariantId = variantDto.ShopifyVariantId,
                        Product = product
                    };

                    await _unitOfWork.Variants.AddAsync(variant);

                    WarehouseInventory inventory = new()
                    {
                        WarehouseId = warehouseId,
                        ProductVariant = variant,
                        Quantity = variantDto.Quantity
                    };

                    await _unitOfWork.Inventories.AddAsync(inventory);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, null, "Inventario creado exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail("Error al guardar el inventario: " + ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> CreateTransferAsync(WarehouseInventoryTransferDto warehouseInventoryTransferDto)
    {
        var newTransfer = _mapper.Map<WarehouseInventoryTransfer>(warehouseInventoryTransferDto);
        newTransfer.Status = TransferStatus.Pending;
        newTransfer.CreatorId = 2; //todo: poner usuario con la sesion
        var newWarehouseInventoryTransfer = await _warehouseInventoryTransferRepository.AddAsync(newTransfer);
        return ApiResponse<bool>.Success(newWarehouseInventoryTransfer.Id > 0, null, "Inventario creado exitosamente");
    }

    // public async Task<ApiResponse<bool>> TryValidateTransferAsync(int transferId)
    // {
    //     var transfer = await _warehouseInventoryTransferRepository.GetByIdWithDetailsAsync(transferId);
    //     if (transfer == null)
    //         return ApiResponse<bool>.Fail("La transferencia no existe.");

    //     foreach (var item in transfer.TransferDetails)
    //     {
    //         var inventory = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.FromWarehouseId, item.ProductVariantId);

    //         if (inventory == null || inventory.Quantity < item.Quantity)
    //         {
    //             return ApiResponse<bool>.Fail($"Inventario insuficiente para el producto variante {item.ProductVariantId}.");
    //         }
    //     }

    //     return ApiResponse<bool>.Success(true);
    // }

    // public async Task<ApiResponse<bool>> AcceptTransferAsync(int transferId, int userId)
    // {
    //     var transfer = await _warehouseInventoryTransferRepository.GetByIdWithDetailsAsync(transferId);
    //     if (transfer == null)
    //         return ApiResponse<bool>.Fail("La transferencia no existe.");

    //     var validationResult = await TryValidateTransferAsync(transferId);
    //     if (!validationResult.IsSuccess)
    //         return validationResult;

    //     // Ejecutar la transferencia
    //     foreach (var item in transfer.TransferDetails)
    //     {
    //         // Restar de origen
    //         var inventoryOrigin = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.FromWarehouseId, item.ProductVariantId);
    //         inventoryOrigin.Quantity -= item.Quantity;
    //         await _unitOfWork.Inventories.UpdateAsync(inventoryOrigin);

    //         // Sumar en destino
    //         var inventoryDestination = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(transfer.ToWarehouseId, item.ProductVariantId);
    //         if (inventoryDestination == null)
    //         {
    //             inventoryDestination = new WarehouseInventory
    //             {
    //                 WarehouseId = transfer.ToWarehouseId,
    //                 ProductVariantId = item.ProductVariantId,
    //                 Quantity = item.Quantity
    //             };
    //             await _unitOfWork.Inventories.AddAsync(inventoryDestination);
    //         }
    //         else
    //         {
    //             inventoryDestination.Quantity += item.Quantity;
    //             await _unitOfWork.Inventories.UpdateAsync(inventoryDestination);
    //         }
    //     }

    //     transfer.Status = TransferStatus.Accepted;
    //     transfer.AcceptedByUserId = userId;
    //     transfer.UpdatedAt = DateTime.UtcNow;
    //     await _unitOfWork.WarehouseInventoryTransfers.UpdateAsync(transfer);

    //     await _unitOfWork.SaveChangesAsync();
    //     return ApiResponse<bool>.Success(true, null, "Transferencia aceptada correctamente.");
    // }

    // public async Task<ApiResponse<bool>> RejectTransferAsync(int transferId, int userId, string rejectionReason)
    // {
    //     var transfer = await _warehouseInventoryTransferRepository.GetByIdWithDetailsAsync(transferId);
    //     if (transfer == null)
    //         return ApiResponse<bool>.Fail("La transferencia no existe.");

    //     transfer.Status = TransferStatus.Rejected;
    //     transfer.AcceptedByUserId = userId;
    //     transfer.UpdatedAt = DateTime.UtcNow;
    //     transfer.RejectionReason = rejectionReason;
    //     await _warehouseInventoryTransferRepository.UpdateAsync(transfer);
    //     return ApiResponse<bool>.Success(true, null, "Transferencia rechazada correctamente.");
    // }

}
