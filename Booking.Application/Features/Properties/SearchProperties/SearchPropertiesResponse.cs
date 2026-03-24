namespace Booking.Application.Features.Properties.SearchProperties;

public sealed record SearchPropertiesResponse(
    List<SearchPropertyItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);