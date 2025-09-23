using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class DeliveryProviderService : IDeliveryProviderService
{
    private readonly IDeliveryProviderRepository _repository;
    private readonly IMapper _mapper;

    public DeliveryProviderService(IDeliveryProviderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<DeliveryProviderDto>>> GetAllAsync(DeliveryProviderFilterDto filter) =>
         ApiResponse<IEnumerable<DeliveryProviderDto>>.Success(_mapper.Map<List<DeliveryProviderDto>>(await _repository.GetAllAsync()));
}
