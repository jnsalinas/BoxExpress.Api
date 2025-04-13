namespace BoxExpress.Domain.Entities;

public class TimeSlot : BaseEntity
{

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public ICollection<Order>? Orders { get; set; } = new List<Order>();
}