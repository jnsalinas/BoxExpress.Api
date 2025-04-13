using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class OrderCategoryService : IOrderCategoryService
{
    private readonly IOrderCategoryRepository _repository;
    private readonly IMapper _mapper;

    public OrderCategoryService(IOrderCategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<OrderCategoryDto>>> GetAllAsync(OrderCategoryFilterDto filter) =>
         ApiResponse<IEnumerable<OrderCategoryDto>>.Success(_mapper.Map<List<OrderCategoryDto>>(await _repository.GetAllAsync()));
}
