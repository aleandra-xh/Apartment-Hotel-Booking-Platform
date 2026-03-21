
using Booking.Domain.PropertySeasonalPrices;

namespace Booking.Application.Abstractions.PropertySeasonalPrices;

public interface IPropertySeasonalPriceRepository
{
    Task<bool> HasOverlappingSeasonalPriceAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default);

    Task<List<PropertySeasonalPrice>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);

    Task<PropertySeasonalPrice?> GetByIdAsync(Guid seasonalPriceId, CancellationToken ct = default);

    Task<decimal?> GetApplicablePricePerNightAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default);
}