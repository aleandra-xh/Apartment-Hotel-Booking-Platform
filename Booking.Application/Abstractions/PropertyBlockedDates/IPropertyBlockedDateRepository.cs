
using Booking.Domain.PropertyBlockedDates;

namespace Booking.Application.Abstractions.PropertyBlockedDates;
public interface IPropertyBlockedDateRepository
{
    Task<bool> HasOverlappingBlockedDateAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default);

    Task<List<PropertyBlockedDate>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default);

    Task<PropertyBlockedDate?> GetByIdAsync(Guid blockedDateId, CancellationToken ct = default);
}