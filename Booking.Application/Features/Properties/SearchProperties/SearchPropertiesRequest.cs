
namespace Booking.Application.Features.Properties.SearchProperties;

public sealed record SearchPropertiesRequest(
    string? City,
    int? MaxGuests,
    int? PropertyType,
    DateTime? StartDate,
    DateTime? EndDate,
    int Page = 1,
    int PageSize = 10
);