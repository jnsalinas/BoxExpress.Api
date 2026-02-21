using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Dtos;

public class AutocompleteVariantFilterDto : BaseFilterDto
{
    public string? Query { get; set; } 
    public int? StoreId { get; set; }  
}