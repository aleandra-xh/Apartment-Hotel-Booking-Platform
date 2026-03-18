
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.GetOwnerReservations;

public sealed record GetOwnerReservationsQuery(
    ReservationStatus? Status,
    bool? IsPast
) : IRequest<List<GetOwnerReservationsResponse>>;