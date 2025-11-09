namespace BoxExpress.Application.Interfaces;

public interface IUserContext
{
    int? UserId { get; }
    int? CountryId { get; }
}