using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseInventoryTransferRepository _warehouseInventoryTransferRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public WarehouseService(
        IWarehouseInventoryTransferRepository warehouseInventoryTransferRepository,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IWarehouseRepository repository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _warehouseInventoryTransferRepository = warehouseInventoryTransferRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
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
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var productDto in products)
            {
                Product product = new()
                {
                    CreatedAt = DateTime.UtcNow,
                    Name = productDto.Name,
                    Sku = productDto.Sku,
                    Price = productDto.Price
                };

                await _unitOfWork.Products.AddAsync(product);

                foreach (var variantDto in productDto.Variants)
                {
                    ProductVariant productVariant = new()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Name = variantDto.Name,
                        ShopifyVariantId = variantDto.ShopifyId,
                        Product = product,
                        Sku = variantDto.Sku,
                        Price = variantDto.Price
                    };

                    await _unitOfWork.Variants.AddAsync(productVariant);

                    await _unitOfWork.Inventories.AddAsync(new()
                    {
                        CreatedAt = DateTime.UtcNow,
                        WarehouseId = warehouseId,
                        ProductVariant = productVariant,
                        Quantity = variantDto.Quantity,
                        StoreId = variantDto.StoreId
                    });

                    await _unitOfWork.InventoryMovements.AddAsync(new InventoryMovement
                    {
                        CreatedAt = DateTime.UtcNow,
                        WarehouseId = warehouseId,
                        ProductVariant = productVariant,
                        Quantity = variantDto.Quantity,
                        MovementType = InventoryMovementType.InitialStock,
                        Notes = "Inventario inicial",
                        Reference = $"Initial-Stock-{product.Name}-{productVariant.Name}"
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return ApiResponse<bool>.Success(true, null, "Inventario creado exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<bool>.Fail("Error al guardar el inventario: " + ex.Message);
        }
    }
}
