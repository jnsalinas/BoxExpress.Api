using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly IProductVariantRepository _repository;
    private readonly IMapper _mapper;
    private readonly IInventoryMovementService _inventoryMovementService;
    public ProductVariantService(IProductVariantRepository repository, IMapper mapper, IInventoryMovementService inventoryMovementService)
    {
        _repository = repository;
        _mapper = mapper;
        _inventoryMovementService = inventoryMovementService;
    }

    public async Task<ApiResponse<List<ProductVariantDto>>> GetAllAsync(ProductVariantFilterDto filter)
    {
        var result = await _repository.GetAllAsync(_mapper.Map<ProductVariantFilter>(filter));
        return ApiResponse<List<ProductVariantDto>>.Success(_mapper.Map<List<ProductVariantDto>>(result));
    }

    public async Task<ApiResponse<ProductVariantDto?>> GetByIdAsync(int id) =>
        ApiResponse<ProductVariantDto?>.Success(_mapper.Map<ProductVariantDto>(await _repository.GetByIdWithDetailsAsync(id)));

         public async Task<ApiResponse<List<ProductVariantDto?>>> GetByNameAsync(string name, int storeId)
    {
        var productVariants = await _repository.GetByVariantNameAndStoreId(name, storeId);
        var response = ApiResponse<List<ProductVariantDto?>>.Success(_mapper.Map<List<ProductVariantDto>>(productVariants));

        if(response.Data.Any())
        {
            foreach (var item in response.Data)
            {
                item.InventoryMovements = (await _inventoryMovementService.GetAllAsync(new InventoryMovementFilterDto()
                {
                    ProductVariantId = item.Id,
                })).Data.ToList();
            }
        }

        return response;
    }

    public async Task<ApiResponse<List<ProductVariantDto>>> GetVariantsAutocompleteAsync(AutocompleteVariantFilterDto filter) =>
        ApiResponse<List<ProductVariantDto>>.Success(_mapper.Map<List<ProductVariantDto>>(await _repository.GetVariantsAutocompleteAsync(_mapper.Map<AutocompleteVariantFilter>(filter))));
}
