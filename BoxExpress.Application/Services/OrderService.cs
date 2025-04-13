using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services;

public class OrderService : IOrderService
{

    private readonly IMapper _mapper;

    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;

    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto filter) =>
             ApiResponse<IEnumerable<OrderDto>>.Success(_mapper.Map<List<OrderDto>>(await _repository.GetFilteredAsync(_mapper.Map<OrderFilter>(filter))));

    public async Task<ApiResponse<OrderDetailDto?>> GetByIdAsync(int id) =>
        ApiResponse<OrderDetailDto?>.Success(_mapper.Map<OrderDetailDto>(await _repository.GetByIdWithDetailsAsync(id)));

    // public async Task<ApiResponse<OrderDto>> AddAsync(OrderDto order)
    // {
    //     // return await _repository.AddAsync(order);
    // }
}
