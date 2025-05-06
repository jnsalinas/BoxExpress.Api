using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class TimeSlotDto : BaseDto
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}