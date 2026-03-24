
namespace Booking.Application.Features.Properties.SearchProperties;

public sealed record SearchPropertyItemResponse(
    Guid Id,
    string Name,
    string Description,
    string PropertyType,
    string City,
    int MaxGuests,
    bool IsActive,
    bool IsApproved
);