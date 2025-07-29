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

    public ProductVariantService(IProductVariantRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<ProductVariantDto>>> GetAllAsync(ProductVariantFilterDto filter)
    {
        return ApiResponse<List<ProductVariantDto>>.Success(_mapper.Map<List<ProductVariantDto>>(await _repository.GetAllAsync(_mapper.Map<ProductVariantFilter>(filter))));
    }

    public async Task<ApiResponse<ProductVariantDto?>> GetByIdAsync(int id) =>
        ApiResponse<ProductVariantDto?>.Success(_mapper.Map<ProductVariantDto>(await _repository.GetByIdWithDetailsAsync(id)));
}
