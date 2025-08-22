namespace BoxExpress.Application.Integrations.Shopify;

public class ShopifyOrderDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public DateTime? Created_At { get; set; }
    public string? Processed_At { get; set; }
    public string? Currency { get; set; }
    public string? Total_Price { get; set; }
    public string? Financial_Status { get; set; }
    public string? Fulfillment_Status { get; set; }
    public string? Order_Status_Url { get; set; }
    public string? Tags { get; set; }
    public string? Note { get; set; }
    public ShopifyCustomerDto? Customer { get; set; }
    public ShopifyShippingAddressDto? Shipping_Address { get; set; }
    public List<ShopifyLineItemDto>? Line_Items { get; set; }
    public List<ShopifyNoteAttributeDto>? Note_Attributes { get; set; }
    public string[]? Payment_Gateway_Names { get; set; }
    public List<ShopifyShippingLineDto>? Shipping_Lines { get; set; }
}