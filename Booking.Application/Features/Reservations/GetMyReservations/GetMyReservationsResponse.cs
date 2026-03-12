

using Booking.Domain.Reservations;

namespace Booking.Application.Features.Reservations.GetMyReservations;

public sealed record GetMyReservationsResponse(
    Guid Id,
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount,
    decimal TotalPrice,
    ReservationStatus BookingStatus,
    DateTime CreatedAt
);

