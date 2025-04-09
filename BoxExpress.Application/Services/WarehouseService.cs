using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;

    public WarehouseService(IWarehouseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllAsync(WarehouseFilterDto filter)
    {
        List<Warehouse> warehouses = await _repository.GetFilteredAsync(_mapper.Map<WarehouseFilter>(filter));
        return _mapper.Map<List<WarehouseDto>>(warehouses);
    }
}
