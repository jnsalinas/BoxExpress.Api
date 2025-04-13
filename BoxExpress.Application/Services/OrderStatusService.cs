using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class OrderStatusService : IOrderStatusService
{
    private readonly IOrderStatusRepository _repository;
    private readonly IMapper _mapper;

    public OrderStatusService(IOrderStatusRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<OrderStatusDto>>> GetAllAsync(OrderStatusFilterDto filter) =>
         ApiResponse<IEnumerable<OrderStatusDto>>.Success(_mapper.Map<List<OrderStatusDto>>(await _repository.GetAllAsync()));
}
