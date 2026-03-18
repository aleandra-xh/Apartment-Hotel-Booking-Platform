using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.GetMyReservations;

public sealed record GetMyReservationsQuery(
    ReservationStatus? Status,
    bool? IsPast
) : IRequest<List<GetMyReservationsResponse>>;