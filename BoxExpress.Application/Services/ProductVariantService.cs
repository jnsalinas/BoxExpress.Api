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

    public async Task<ApiResponse<List<ProductVariantAutocompleteDto>>> GetVariantsAutocompleteAsync(string query) =>
         ApiResponse<List<ProductVariantAutocompleteDto>>.Success(_mapper.Map<List<ProductVariantAutocompleteDto>>(await _repository.GetVariantsAutocompleteAsync(query)));
}
