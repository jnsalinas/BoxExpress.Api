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

    public async Task<ApiResponse<bool>> TransferInventoryAsync(WarehouseInventoryTransferDto warehouseInventoryTransferDto)
    {
        //FromWarehouseId
        //ProductVariantId
        var fromInventory = await _warehouseInventoryRepository
            .GetByWarehouseIdAndProductVariantId(warehouseInventoryTransferDto.FromWarehouseId, warehouseInventoryTransferDto.ProductVariantId);

        if (fromInventory == null || fromInventory.Quantity < warehouseInventoryTransferDto.Quantity)
            return ApiResponse<bool>.Fail("No hay suficiente inventario en el almacén de origen.");

        var toInventory = await _warehouseInventoryRepository
            .GetByWarehouseIdAndProductVariantId(warehouseInventoryTransferDto.ToWarehouseId, warehouseInventoryTransferDto.ProductVariantId);

        // Resta del origen
        fromInventory.Quantity -= warehouseInventoryTransferDto.Quantity;

        // Suma al destino
        if (toInventory != null)
        {
            toInventory.Quantity += warehouseInventoryTransferDto.Quantity;
        }
        else
        {
            await _warehouseInventoryRepository.AddAsync(new WarehouseInventory
            {
                WarehouseId = warehouseInventoryTransferDto.ToWarehouseId,
                ProductVariantId = warehouseInventoryTransferDto.ProductVariantId,
                Quantity = warehouseInventoryTransferDto.Quantity
            });
        }

        // Registrar la transferencia
        await _warehouseInventoryTransferRepository.AddAsync(new WarehouseInventoryTransfer
        {
            FromWarehouseId = warehouseInventoryTransferDto.FromWarehouseId,
            ToWarehouseId = warehouseInventoryTransferDto.ToWarehouseId,
            ProductVariantId = warehouseInventoryTransferDto.ProductVariantId,
            Quantity = warehouseInventoryTransferDto.Quantity,
            CreatedAt = DateTime.UtcNow
        });

        return new ApiResponse<bool>
        {
            IsSuccess = true,
            Message = "Transferencia de inventario exitosa"
        };
    }


    // public async Task<ApiResponse<bool>> UpdateInventoryInWarehouseAsync(int warehouseId, List<CreateProductWithVariantsDto> products)
    // {
    //     await _unitOfWork.BeginTransactionAsync(); // Inicia la transacción

    //     try
    //     {
    //         foreach (var productDto in products)
    //         {
    //             var product = await _unitOfWork.Products.GetByIdAsync(productDto.Id.Value);
    //             if (product == null)
    //             {
    //                 return ApiResponse<bool>.Fail($"Producto con ID {productDto.Id} no encontrado.");
    //             }

    //             // Actualizar detalles del producto
    //             product.Name = productDto.Name;
    //             product.ShopifyProductId = productDto.ShopifyProductId;
    //             await _unitOfWork.Products.UpdateAsync(product);

    //             // Cargar variantes e inventarios existentes
    //             var existingVariants = await _unitOfWork.Variants.GetByProductIdAsync(product.Id);
    //             var existingInventories = await _unitOfWork.Inventories.GetByWarehouseIdAsync(warehouseId);

    //             // Eliminar variantes que ya no están
    //             foreach (var existingVariant in existingVariants)
    //             {
    //                 if (!productDto.Variants.Any(v => v.Id == existingVariant.Id))
    //                 {
    //                     await _unitOfWork.Variants.DeleteAsync(existingVariant);

    //                     // Eliminar inventario asociado
    //                     var inventory = existingInventories.FirstOrDefault(i => i.ProductVariantId == existingVariant.Id);
    //                     if (inventory != null)
    //                     {
    //                         await _unitOfWork.Inventories.DeleteAsync(inventory);
    //                     }
    //                 }
    //             }

    //             // Procesar las variantes (actualizar o agregar)
    //             foreach (var variantDto in productDto.Variants)
    //             {
    //                 var variant = existingVariants.FirstOrDefault(v => v.Id == variantDto.Id);
    //                 if (variant == null)
    //                 {
    //                     // Crear nueva variante si no existe
    //                     variant = new ProductVariant
    //                     {
    //                         Name = variantDto.Name,
    //                         ShopifyVariantId = variantDto.ShopifyVariantId,
    //                         Product = product
    //                     };
    //                     await _unitOfWork.Variants.AddAsync(variant);
    //                 }
    //                 else
    //                 {
    //                     // Actualizar variante existente
    //                     variant.Name = variantDto.Name;
    //                     variant.ShopifyVariantId = variantDto.ShopifyVariantId;
    //                     await _unitOfWork.Variants.UpdateAsync(variant);
    //                 }

    //                 // Actualizar o agregar inventarios
    //                 var inventory = existingInventories.FirstOrDefault(i => i.ProductVariantId == variant.Id);
    //                 if (inventory == null)
    //                 {
    //                     inventory = new WarehouseInventory
    //                     {
    //                         WarehouseId = warehouseId,
    //                         ProductVariant = variant,
    //                         Quantity = variantDto.Quantity
    //                     };
    //                     await _unitOfWork.Inventories.AddAsync(inventory);
    //                 }
    //                 else
    //                 {
    //                     inventory.Quantity = variantDto.Quantity;
    //                     await _unitOfWork.Inventories.UpdateAsync(inventory);
    //                 }
    //             }
    //         }

    //         await _unitOfWork.SaveChangesAsync(); // Guarda los cambios
    //         await _unitOfWork.CommitAsync(); // Confirma la transacción
    //         return ApiResponse<bool>.Success(true, "Inventario actualizado exitosamente");
    //     }
    //     catch (Exception ex)
    //     {
    //         await _unitOfWork.RollbackAsync(); // Reversa la transacción si hay un error
    //         return ApiResponse<bool>.Fail("Error al actualizar el inventario: " + ex.Message);
    //     }
    // }



}
