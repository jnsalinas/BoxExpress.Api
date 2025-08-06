namespace BoxExpress.Domain.Entities;

public class ProductLoanDetail : BaseEntity
{
    public int ProductLoanId { get; set; }
    public ProductLoan ProductLoan { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int RequestedQuantity { get; set; }
    public int DeliveredQuantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public int PendingReturnQuantity => DeliveredQuantity - ReturnedQuantity;
}