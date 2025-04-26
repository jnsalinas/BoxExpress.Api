using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _repository;
    private readonly IMapper _mapper;

    public StoreService(IStoreRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<StoreDto>>> GetAllAsync(StoreFilterDto filter) =>
         ApiResponse<IEnumerable<StoreDto>>.Success(_mapper.Map<List<StoreDto>>(await _repository.GetAllAsync()));
}
