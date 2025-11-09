using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Filters;

public class CityFilter : BaseFilter
{
    public int? CountryId { get; set; }
} 