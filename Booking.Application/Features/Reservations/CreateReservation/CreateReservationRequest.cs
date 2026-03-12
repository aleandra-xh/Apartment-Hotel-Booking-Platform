
namespace Booking.Application.Features.Reservations.CreateReservation;

public sealed record CreateReservationRequest(
    Guid PropertyId,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount
);