
namespace Booking.Application.Features.PropertyDiscounts.AddPropertyDiscount;

public sealed record AddPropertyDiscountRequest(
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    decimal Percentage,
    string? Label
);