namespace BoxExpress.Application.Dtos
{
    public class BulkChangeOrdersStatusDto : ChangeStatusDto
    {
        public List<int> OrderIds { get; set; } = new List<int>();
        public int StatusId { get; set; }
    }
}
