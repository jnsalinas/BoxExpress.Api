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
        try
        {
            foreach (var productDto in products)
            {
                Product product = new()
                {
                    CreatedAt = DateTime.UtcNow,
                    Name = productDto.Name,
                    ShopifyProductId = productDto.ShopifyProductId,
                    StoreId = 1 // todo: revisar si cada producto tiene una tienda asociada
                };

                await _unitOfWork.Products.AddAsync(product);

                foreach (var variantDto in productDto.Variants)
                {
                    ProductVariant variant = new()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Name = variantDto.Name,
                        ShopifyVariantId = variantDto.ShopifyVariantId,
                        Product = product
                    };

                    await _unitOfWork.Variants.AddAsync(variant);

                    WarehouseInventory inventory = new()
                    {
                        CreatedAt = DateTime.UtcNow,
                        WarehouseId = warehouseId,
                        ProductVariant = variant,
                        Quantity = variantDto.Quantity
                    };

                    await _unitOfWork.Inventories.AddAsync(inventory);
                    await _unitOfWork.InventoryMovements.AddAsync(new InventoryMovement
                    {
                        CreatedAt = DateTime.UtcNow,
                        WarehouseId = warehouseId,
                        ProductVariantId = variant.Id,
                        Quantity = variantDto.Quantity,
                        MovementType = InventoryMovementType.InitialStock,
                        Notes = "Inventario inicial",
                        Reference = $"Initial-Stock-{product.Id}-{variant.Id}"
                    });
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
        newTransfer.CreatedAt = DateTime.UtcNow;
        newTransfer.Status = InventoryTransferStatus.Pending;
        newTransfer.CreatorId = 2; //todo: poner usuario con la sesion
        var newWarehouseInventoryTransfer = await _warehouseInventoryTransferRepository.AddAsync(newTransfer);
        return ApiResponse<bool>.Success(newWarehouseInventoryTransfer.Id > 0, null, "Inventario creado exitosamente");
    }
}
