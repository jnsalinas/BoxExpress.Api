using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface ITimeSlotService
{
    Task<ApiResponse<IEnumerable<TimeSlotDto>>> GetAllAsync(TimeSlotFilterDto filter);
}
