using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class InventoryHoldService : IInventoryHoldService
{
    private readonly IInventoryHoldRepository _repository;
    private readonly IMapper _mapper;

    public InventoryHoldService(IInventoryHoldRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<InventoryHoldDto>>> GetAllAsync(InventoryHoldFilterDto filter)
    {
        var (InventoryHolds, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<InventoryHoldFilter>(filter));
        return ApiResponse<IEnumerable<InventoryHoldDto>>.Success(_mapper.Map<List<InventoryHoldDto>>(InventoryHolds), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }
}
