

namespace Booking.Application.Features.PropertyBlockedDates.GetPropertyBlockedDates;

public sealed record GetPropertyBlockedDatesResponse(
    Guid Id,
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason,
    DateTime CreatedAt
);