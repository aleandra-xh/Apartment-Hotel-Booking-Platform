
using Booking.Domain.Reservations;

namespace Booking.Application.Features.Reservations.GetAllReservationsForAdmin;
public sealed record GetAllReservationsForAdminResponse(
    Guid Id,
    Guid PropertyId,
    Guid GuestId,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount,
    decimal TotalPrice,
    ReservationStatus BookingStatus,
    DateTime CreatedAt
);