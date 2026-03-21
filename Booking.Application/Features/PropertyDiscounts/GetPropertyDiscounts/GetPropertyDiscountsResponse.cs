
namespace Booking.Application.Features.PropertyDiscounts.GetPropertyDiscounts;

public sealed record GetPropertyDiscountsResponse(
    Guid Id,
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    decimal Percentage,
    string? Label,
    DateTime CreatedAt
);