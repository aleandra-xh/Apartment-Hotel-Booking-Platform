

namespace Booking.Application.Abstractions.Reservations;

public interface IReservationRepository
{
    Task<bool> HasOverlappingReservationAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default);
}