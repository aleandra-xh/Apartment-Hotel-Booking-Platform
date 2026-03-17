using Booking.Domain.Properties;

namespace Booking.Application.Abstractions.Properties;

public interface IPropertyRepository
{
    Task<Property?> GetPropertyByIdWithDetailsAsync(Guid propertyId, CancellationToken ct = default);

    Task<(List<Property> Items, int TotalCount)> SearchPropertiesAsync(
        string? city,
        int? maxGuests,
        int? propertyType,
        DateTime? startDate,
        DateTime? endDate,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<Property?> GetPropertyForReservationAsync(Guid propertyId, CancellationToken ct = default);
}