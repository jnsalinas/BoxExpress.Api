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
    private readonly IOrderCategoryRepository _orderCategoryRepository;

    public OrderService(IOrderRepository repository, IMapper mapper, IOrderCategoryRepository orderCategoryRepository)
    {
        _orderCategoryRepository = orderCategoryRepository;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto filter) =>
             ApiResponse<IEnumerable<OrderDto>>.Success(_mapper.Map<List<OrderDto>>(await _repository.GetFilteredAsync(_mapper.Map<OrderFilter>(filter))));

    public async Task<ApiResponse<OrderDetailDto?>> GetByIdAsync(int id) =>
        ApiResponse<OrderDetailDto?>.Success(_mapper.Map<OrderDetailDto>(await _repository.GetByIdWithDetailsAsync(id)));

    public async Task<ApiResponse<OrderDto>> UpdateWarehouseAsync(int orderId, int warehouseId)
    {
        List<OrderCategory> categories = await _orderCategoryRepository.GetAllAsync();
        Order? order = await _repository.GetByIdAsync(orderId);

        if (order == null)
            return ApiResponse<OrderDto>.Fail("Order not found");

        if (warehouseId.Equals(0))
            order.OrderCategoryId = categories.First(x => x.Name.Equals("Tradicional")).Id;
        else
        {
            order.OrderCategoryId = categories.First(x => x.Name.Equals("Express")).Id;
            order.WarehouseId = warehouseId;
        }

        //todo guardar log de cambio de categoria
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}
