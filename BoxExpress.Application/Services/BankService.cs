using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class BankService : IBankService
{
    private readonly IBankRepository _repository;
    private readonly IMapper _mapper;

    public BankService(IBankRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BankDto>>> GetAllAsync(BankFilterDto filter) =>
         ApiResponse<IEnumerable<BankDto>>.Success(_mapper.Map<List<BankDto>>(await _repository.GetAllAsync()));
}
