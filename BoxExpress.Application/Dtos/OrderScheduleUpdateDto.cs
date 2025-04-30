namespace BoxExpress.Application.Dtos;

public class OrderScheduleUpdateDto
{
    public int? StatusId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int? TimeSlotId { get; set; }
}
