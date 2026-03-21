
namespace Booking.Application.Features.PropertySeasonalPrices.AddSeasonalPrice;

public sealed record AddSeasonalPriceRequest(
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    decimal PricePerNight,
    string? Label
);