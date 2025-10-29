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
    private readonly IUserContext _userContext;

    public WarehouseInventoryService(IWarehouseInventoryRepository repository, IMapper mapper, IInventoryMovementService inventoryMovementService, IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _inventoryMovementService = inventoryMovementService;
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<ApiResponse<IEnumerable<ProductVariantDto>>> GetAllAsync(WarehouseInventoryFilterDto filter)
    {
        var (productVariants, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WarehouseInventoryFilter>(filter));
        return ApiResponse<IEnumerable<ProductVariantDto>>.Success(_mapper.Map<List<ProductVariantDto>>(productVariants), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetWarehouseProductSummaryGroupAsync(WarehouseInventoryFilterDto filter)
    {
        filter.IsAll = true;
        var result = await GetAllAsync(filter);

        var groupedData = result.Data
            .GroupBy(x => new { x.StoreId, x.ProductName })
            .Select(group => new ProductDto()
            {
                TotalQuantity = group.Sum(x => x.Quantity),
                Name = group.Key.ProductName ?? string.Empty,
                Variants = group
                    .GroupBy(v => new { v.Name, v.Sku, v.ShopifyVariantId, v.Price })
                    .Select(variantGroup => new ProductVariantDto
                    {
                        Name = variantGroup.Key.Name,
                        Sku = variantGroup.Key.Sku,
                        ShopifyVariantId = variantGroup.Key.ShopifyVariantId,
                        Price = variantGroup.Key.Price,
                        Quantity = variantGroup.Sum(q => q.Quantity),
                        PendingReturnQuantity = variantGroup.Sum(q => q.PendingReturnQuantity),
                        AvailableQuantity = variantGroup.Sum(q => q.AvailableQuantity),
                        BlockedQuantity = variantGroup.Sum(q => q.BlockedQuantity),
                        ReservedQuantity = variantGroup.Sum(q => q.ReservedQuantity),
                        DeliveredQuantity = variantGroup.Sum(q => q.DeliveredQuantity),
                        OnTheWayQuantity = variantGroup.Sum(q => q.OnTheWayQuantity),
                    }).ToList()
            }).OrderBy(x => x.Name).ToList();

        return ApiResponse<IEnumerable<ProductDto>>.Success(groupedData, result.Pagination);
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetWarehouseProductSummaryAsync(WarehouseInventoryFilterDto filter)
    {
        filter.CountryId = _userContext?.CountryId != null ? _userContext.CountryId : filter.CountryId;
        var (products, totalCount) = await _repository.GetFilteredGroupedByProductAsync(_mapper.Map<WarehouseInventoryFilter>(filter));
        var variants = await _repository.GetByWarehouseAndProductsId(filter.WarehouseId, products.Select(p => p.Id).ToList(), new WarehouseInventoryFilter()
        {
            StoreId = filter.StoreId,
        });

        //todo mirar si se puede pasar a automapper 
        var groupedProducts = products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            ShopifyProductId = product.ShopifyProductId,
            Price = product.Price,
            Sku = product.Sku,
            TotalQuantity = variants.Where(v => v.ProductVariant.ProductId == product.Id).Sum(v => v.Quantity),
            Variants = variants
                .Where(v => v.ProductVariant.ProductId == product.Id)
                .Select(wi => new ProductVariantDto
                {
                    warehouseId = wi.WarehouseId,
                    WarehouseName = wi.Warehouse?.Name ?? "",
                    Name = wi.ProductVariant.Name ?? "",
                    ShopifyVariantId = wi.ProductVariant.ShopifyVariantId,
                    Sku = wi.ProductVariant.Sku,
                    ReservedQuantity = wi.ReservedQuantity,
                    AvailableQuantity = wi.AvailableQuantity,
                    BlockedQuantity = wi.BlockedQuantity,
                    Id = wi.ProductVariant.Id,
                    WarehouseInventoryId = wi.Id,
                    Price = wi.ProductVariant.Price,
                    Quantity = wi.Quantity,
                    PendingReturnQuantity = wi.PendingReturnQuantity,
                    OnTheWayQuantity = wi.OnTheWayQuantity,
                    StoreId = wi.StoreId,
                    DeliveredQuantity = wi.DeliveredQuantity ?? 0,
                    Store = wi.Store != null ? new StoreDto { Id = wi.Store.Id, Name = wi.Store.Name } : null,
                    ProductName = product.Name,
                }).OrderBy(x => x.Store?.Name).ToList()
        }).OrderBy(x => x.Name)
        .ToList();

        return ApiResponse<IEnumerable<ProductDto>>.Success(groupedProducts, new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<List<ProductVariantAutocompleteDto>>> GetVariantsAutocompleteAsync(string query, int WarehouseOriginId) =>
         ApiResponse<List<ProductVariantAutocompleteDto>>.Success(_mapper.Map<List<ProductVariantAutocompleteDto>>(await _repository.GetVariantsAutocompleteAsync(query, WarehouseOriginId)));

    public async Task<ApiResponse<WarehouseInventoryDto?>> GetByIdAsync(int id) =>
           ApiResponse<WarehouseInventoryDto?>.Success(_mapper.Map<WarehouseInventoryDto>(await _repository.GetByIdWithDetailsAsync(id)));

    public async Task<ApiResponse<WarehouseInventoryDto?>> UpdateAsync(int id, UpdateWarehouseInventoryDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var warehouseInventory = await _repository.GetByIdWithDetailsAsync(id);
            if (warehouseInventory == null)
                return ApiResponse<WarehouseInventoryDto?>.Fail("Warehouse inventory not found.");

            if (!string.IsNullOrEmpty(dto.VariantSku))
            {
                var existSKU = await _repository.GetBySkusAsync(new HashSet<string> { dto.VariantSku });
                if (existSKU != null && existSKU.Any() && existSKU.Any(x => x.Id != id && x.ProductVariant.ProductId != warehouseInventory.ProductVariant.ProductId))
                    return ApiResponse<WarehouseInventoryDto?>.Fail("SKU ya existe");
            }

            await _unitOfWork.BeginTransactionAsync();

            int quantityDifference = 0;
            //register inventory movement
            if (dto.Quantity != null && warehouseInventory.Quantity != dto.Quantity)
            {
                quantityDifference = dto.Quantity.Value - warehouseInventory.Quantity;
            }

            if (dto.AddQuantity != null && dto.AddQuantity > 0)
            {
                quantityDifference = dto.AddQuantity.Value;
            }

            if (quantityDifference != 0)
            {
                await _inventoryMovementService.AdjustInventoryAsync(new InventoryMovement
                {
                    WarehouseId = warehouseInventory.WarehouseId,
                    ProductVariantId = warehouseInventory.ProductVariantId,
                    Quantity = quantityDifference,
                    MovementType = InventoryMovementType.ManualAdjustment,
                    Notes = dto.Notes,
                    Reference = "Adjustment",
                }, false);
            }

            if (dto.VariantSku != null)
                warehouseInventory.ProductVariant.Sku = dto.VariantSku;

            if (dto.VariantName != null)
                warehouseInventory.ProductVariant.Name = dto.VariantName;

            if (dto.ShopifyVariantId != null)
                warehouseInventory.ProductVariant.ShopifyVariantId = dto.ShopifyVariantId;



            if (dto.Price != null)
                warehouseInventory.ProductVariant.Price = dto.Price;

            await _unitOfWork.Variants.UpdateAsync(warehouseInventory.ProductVariant);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            if (dto.StoreId.HasValue) //todo optimizar esto
            {
                warehouseInventory.StoreId = dto.StoreId;
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Inventories.UpdateAsync(warehouseInventory);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }


            return ApiResponse<WarehouseInventoryDto?>.Success(_mapper.Map<WarehouseInventoryDto>(warehouseInventory));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<WarehouseInventoryDto?>.Fail($"Error updating warehouse inventory: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ManageOnTheWayInventoryAsync(int warehouseId, List<OrderItem> orderItems)
    {
        var warehouseInventory = await _repository.GetByWarehouseAndProductVariants(warehouseId, orderItems.Select(x => x.ProductVariantId).ToList());
        foreach (var item in warehouseInventory)
        {
            item.OnTheWayQuantity += orderItems.First(x => x.ProductVariantId == item.ProductVariantId).Quantity;
            item.ReservedQuantity -= orderItems.First(x => x.ProductVariantId == item.ProductVariantId).Quantity;
            await _repository.UpdateAsync(item);
        }
        return ApiResponse<bool>.Success(true);
    }
}
