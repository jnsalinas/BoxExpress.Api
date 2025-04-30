namespace BoxExpress.Domain.Entities;

public class User : BaseEntity
{
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public string? PickupAddress { get; set; }
    public string? CompanyName { get; set; }
    public string? LegalName { get; set; }
    public string? DocumentNumber { get; set; }

    public string? FullName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
                return null;

            return $"{FirstName} {LastName}".Trim();
        }
    }
}