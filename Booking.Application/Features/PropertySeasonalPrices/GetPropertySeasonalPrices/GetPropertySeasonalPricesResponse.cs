
namespace Booking.Application.Features.PropertySeasonalPrices.GetPropertySeasonalPrices;

public sealed record GetPropertySeasonalPricesResponse(
    Guid Id,
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    decimal PricePerNight,
    string? Label,
    DateTime CreatedAt
);