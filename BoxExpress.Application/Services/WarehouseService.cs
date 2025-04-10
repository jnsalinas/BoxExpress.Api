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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;

    public WarehouseService(IWarehouseRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IEnumerable<WarehouseDto>>> GetAllAsync(WarehouseFilterDto filter)
    {
        List<Warehouse> warehouses = await _repository.GetFilteredAsync(_mapper.Map<WarehouseFilter>(filter));
        return ApiResponse<IEnumerable<WarehouseDto>>.Success(_mapper.Map<List<WarehouseDto>>(warehouses));
    }

    public async Task<ApiResponse<WarehouseDetailDto?>> GetByIdAsync(int id)
    {
        Warehouse? warehouses = await _repository.GetByIdWithDetailsAsync(id);
        return ApiResponse<WarehouseDetailDto?>.Success(_mapper.Map<WarehouseDetailDto>(warehouses));
        ;
    }

    // UnitOfWork es un patrón que coordina varios repositorios para que puedas hacer múltiples operaciones sobre la base de datos en una única transacción.
    // ✅ Su objetivo es garantizar que todo se guarde o nada se guarde (atomicidad).
    public async Task<ApiResponse<bool>> AddInventoryToWarehouseAsync(int warehouseId, List<CreateProductWithVariantsDto> products)
    {
        try
        {
            foreach (var productDto in products)
            {
                Product product = new Product
                {
                    Name = productDto.Name,
                    ShopifyProductId = productDto.ShopifyProductId,
                    StoreId = 1 // todo revisar si cada producto tiene una tienda asociada
                };

                await _unitOfWork.Products.AddAsync(product);

                foreach (var variantDto in productDto.Variants)
                {
                    ProductVariant variant = new ProductVariant
                    {
                        Name = variantDto.Name,
                        ShopifyVariantId = variantDto.ShopifyVariantId,
                        Product = product
                    };

                    await _unitOfWork.Variants.AddAsync(variant);

                    WarehouseInventory inventory = new WarehouseInventory
                    {
                        WarehouseId = warehouseId,
                        ProductVariant = variant,
                        Quantity = variantDto.Quantity
                    };

                    await _unitOfWork.Inventories.AddAsync(inventory);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Inventario creado exitosamente");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure("Error al guardar el inventario: " + ex.Message);
        }

    }
}
