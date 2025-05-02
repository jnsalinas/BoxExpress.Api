using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class WithdrawalRequestService : IWithdrawalRequestService
{
    private readonly IWithdrawalRequestRepository _repository;
    private readonly IMapper _mapper;

    public WithdrawalRequestService(IWithdrawalRequestRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WithdrawalRequestDto>>> GetAllAsync(WithdrawalRequestFilterDto filter)
    {
        var (withdrawalRequest, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WithdrawalRequestFilter>(filter));
        return ApiResponse<IEnumerable<WithdrawalRequestDto>>.Success(_mapper.Map<List<WithdrawalRequestDto>>(withdrawalRequest), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<bool>> AddAsync(WithdrawalRequestDto dto)
    {
        await _repository.AddAsync(_mapper.Map<WithdrawalRequest>(dto));
        return ApiResponse<bool>.Success(true);
    }
}
