
namespace Booking.Application.Features.Properties.SearchProperties;

public sealed record SearchPropertiesRequest(
    string? City,
    int? MaxGuests,
    int? PropertyType,
    int Page = 1,
    int PageSize = 20
);