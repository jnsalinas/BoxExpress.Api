using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class WarehouseInventoryService : IWarehouseInventoryService
{
    private readonly IWarehouseInventoryRepository _repository;
    private readonly IMapper _mapper;

    public WarehouseInventoryService(IWarehouseInventoryRepository repository, IMapper mapper)
    {
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

        var groupedProducts = products.Select(product => new ProductDto
        {
            Name = product.Name,
            ShopifyId = product.ShopifyProductId,
            Price = product.Price,
            Sku = product.Sku,
            Variants = variants
                .Where(v => v.ProductVariant.ProductId == product.Id)
                .Select(wi => new ProductVariantDto
                {
                    Name = wi.ProductVariant.Name ?? "",
                    ShopifyId = wi.ProductVariant.ShopifyVariantId,
                    Sku = wi.ProductVariant.Sku,
                    ReservedQuantity = wi.ReservedQuantity,
                    AvailableQuantity = wi.AvailableQuantity,
                    Id = wi.ProductVariant.Id,
                    WarehouseInventoryId = wi.Id,
                    Price = wi.ProductVariant.Price,
                    Quantity = wi.Quantity
                }).ToList()
        }).ToList();

        return ApiResponse<IEnumerable<ProductDto>>.Success(groupedProducts, new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<List<ProductVariantAutocompleteDto>>> GetVariantsAutocompleteAsync(string query, int WarehouseOriginId) =>
         ApiResponse<List<ProductVariantAutocompleteDto>>.Success(_mapper.Map<List<ProductVariantAutocompleteDto>>(await _repository.GetVariantsAutocompleteAsync(query, WarehouseOriginId)));

    public async Task<ApiResponse<WarehouseInventoryDto?>> GetByIdAsync(int id) =>
           ApiResponse<WarehouseInventoryDto?>.Success(_mapper.Map<WarehouseInventoryDto>(await _repository.GetByIdWithDetailsAsync(id)));
}
