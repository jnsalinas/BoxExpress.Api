namespace BoxExpress.Application.Dtos;

public class TimeSlotDto
{
    public int Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}