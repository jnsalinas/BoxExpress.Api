using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _repository;
    private readonly IMapper _mapper;

    public TimeSlotService(ITimeSlotRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<TimeSlotDto>>> GetAllAsync(TimeSlotFilterDto filter) =>
         ApiResponse<IEnumerable<TimeSlotDto>>.Success(_mapper.Map<List<TimeSlotDto>>(await _repository.GetAllAsync()));
}
