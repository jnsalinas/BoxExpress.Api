using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Services;

public class WarehouseInventoryService : IWarehouseInventoryService
{
    private readonly IWarehouseInventoryRepository _repository;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;


    public WarehouseInventoryService(IWarehouseInventoryRepository repository, IMapper mapper, IInventoryMovementService inventoryMovementService, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _inventoryMovementService = inventoryMovementService;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<ProductVariantDto>>> GetAllAsync(WarehouseInventoryFilterDto filter)
    {
        var (productVariants, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WarehouseInventoryFilter>(filter));
        return ApiResponse<IEnumerable<ProductVariantDto>>.Success(_mapper.Map<List<ProductVariantDto>>(productVariants), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetWarehouseProductSummaryAsync(WarehouseInventoryFilterDto filter)
    {
        var (products, totalCount) = await _repository.GetFilteredGroupedByProductAsync(_mapper.Map<WarehouseInventoryFilter>(filter));
        var variants = await _repository.GetByWarehouseAndProductsId(filter.WarehouseId, products.Select(p => p.Id).ToList());

        //todo mirar si se puede pasar a automapper 
        var groupedProducts = products.Select(product => new ProductDto
        {
            Name = product.Name,
            ShopifyProductId = product.ShopifyProductId,
            Price = product.Price,
            Sku = product.Sku,
            Variants = variants
                .Where(v => v.ProductVariant.ProductId == product.Id)
                .Select(wi => new ProductVariantDto
                {
                    Name = wi.ProductVariant.Name ?? "",
                    ShopifyVariantId = wi.ProductVariant.ShopifyVariantId,
                    Sku = wi.ProductVariant.Sku,
                    ReservedQuantity = wi.ReservedQuantity,
                    AvailableQuantity = wi.AvailableQuantity,
                    Id = wi.ProductVariant.Id,
                    WarehouseInventoryId = wi.Id,
                    Price = wi.ProductVariant.Price,
                    Quantity = wi.Quantity,
                    PendingReturnQuantity = wi.PendingReturnQuantity,
                }).ToList()
        }).ToList();

        return ApiResponse<IEnumerable<ProductDto>>.Success(groupedProducts, new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<List<ProductVariantAutocompleteDto>>> GetVariantsAutocompleteAsync(string query, int WarehouseOriginId) =>
         ApiResponse<List<ProductVariantAutocompleteDto>>.Success(_mapper.Map<List<ProductVariantAutocompleteDto>>(await _repository.GetVariantsAutocompleteAsync(query, WarehouseOriginId)));

    public async Task<ApiResponse<WarehouseInventoryDto?>> GetByIdAsync(int id) =>
           ApiResponse<WarehouseInventoryDto?>.Success(_mapper.Map<WarehouseInventoryDto>(await _repository.GetByIdWithDetailsAsync(id)));

    public async Task<ApiResponse<WarehouseInventoryDto?>> UpdateAsync(int id, UpdateWarehouseInventoryDto dto)
    {
        var warehouseInventory = await _repository.GetByIdWithDetailsAsync(id);
        if (warehouseInventory == null)
            return ApiResponse<WarehouseInventoryDto?>.Fail("Warehouse inventory not found.");

        await _unitOfWork.BeginTransactionAsync();

        //register inventory movement
        if (warehouseInventory.Quantity != dto.Quantity)
        {
            int quantityDifference = dto.Quantity - warehouseInventory.Quantity;
            await _inventoryMovementService.AdjustInventoryAsync(new InventoryMovement
            {
                WarehouseId = warehouseInventory.WarehouseId,
                ProductVariantId = warehouseInventory.ProductVariantId,
                Quantity = quantityDifference,
                MovementType = InventoryMovementType.ManualAdjustment,
                Notes = dto.Notes,
                Reference = "Adjustment"
            }, false); //todo mirar si es necesario el moveReserved o hacer otra funcion
        }

        if (dto.ProductName != null)
            warehouseInventory.ProductVariant.Product.Name = dto.ProductName;

        if (dto.ProductSku != null)
            warehouseInventory.ProductVariant.Product.Sku = dto.ProductSku;

        if (dto.ShopifyProductId != null)
            warehouseInventory.ProductVariant.Product.ShopifyProductId = dto.ShopifyProductId;

        if (dto.VariantSku != null)
            warehouseInventory.ProductVariant.Sku = dto.VariantSku;

        if (dto.VariantName != null)
            warehouseInventory.ProductVariant.Name = dto.VariantName;

        if (dto.ShopifyVariantId != null)
            warehouseInventory.ProductVariant.ShopifyVariantId = dto.ShopifyVariantId;

        await _unitOfWork.Inventories.UpdateAsync(warehouseInventory);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        return ApiResponse<WarehouseInventoryDto?>.Success(_mapper.Map<WarehouseInventoryDto>(warehouseInventory));
    }
}
