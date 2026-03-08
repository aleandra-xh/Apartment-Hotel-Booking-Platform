
namespace Booking.Application.Features.Properties.GetMyProperties;

public sealed record GetMyPropertiesResponse
(
    Guid Id,
    string Name,
    string Description,
    string PropertyType,
    int MaxGuests,
    bool IsActive,
    bool IsApproved

);
