
namespace Booking.Application.Features.Properties.SearchProperties;

public sealed record SearchPropertiesRequest(
    string? City,
    int? MaxGuests,
    int? PropertyType,
    DateTime? StartDate,
    DateTime? EndDate,
    decimal? MinPrice,
    decimal? MaxPrice,
    List<int>? AmenityIds,
    double? MinRating,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 10
);
