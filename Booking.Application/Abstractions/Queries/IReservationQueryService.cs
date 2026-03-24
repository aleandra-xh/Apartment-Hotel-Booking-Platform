
using Booking.Application.Features.Reservations.GetMyReservations;
using Booking.Application.Features.Reservations.GetOwnerReservations;
using Booking.Domain.Reservations;

namespace Booking.Application.Abstractions.Queries;

public interface IReservationQueryService
{
    Task<GetMyReservationsResult> GetMyReservationsAsync(
        Guid guestId,
        ReservationStatus? status,
        bool? isPast,
        int page,
        int pageSize,
        CancellationToken ct);

    Task<GetOwnerReservationsResult> GetOwnerReservationsAsync(
        Guid ownerId,
        ReservationStatus? status,
        bool? isPast,
        int page,
        int pageSize,
        CancellationToken ct);
}