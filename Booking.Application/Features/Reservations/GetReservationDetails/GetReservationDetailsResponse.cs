
using Booking.Domain.Reservations;

namespace Booking.Application.Features.Reservations.GetReservationDetails;
public sealed record GetReservationDetailsResponse(
    Guid Id,
    Guid PropertyId,
    Guid GuestId,
    DateTime StartDate,
    DateTime EndDate,
    int GuestCount,
    decimal PriceForPeriod,
    decimal CleaningFee,
    decimal AdditionalGuestFee,
    decimal ServiceFee,
    decimal TaxAmount,
    decimal TotalPrice,
    ReservationStatus BookingStatus,
    DateTime CreatedAt
);