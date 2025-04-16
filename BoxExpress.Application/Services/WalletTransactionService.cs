using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class WalletTransactionService : IWalletTransactionService
{
    private readonly IWalletTransactionRepository _repository;
    private readonly IMapper _mapper;

    public WalletTransactionService(IWalletTransactionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WalletTransactionDto>>> GetAllAsync(WalletTransactionFilterDto filter) =>
         ApiResponse<IEnumerable<WalletTransactionDto>>.Success(_mapper.Map<List<WalletTransactionDto>>(await _repository.GetFilteredAsync(_mapper.Map<WalletTransactionFilter>(filter))));
}
