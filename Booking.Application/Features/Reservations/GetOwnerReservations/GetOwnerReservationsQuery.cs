
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.GetOwnerReservations;

public sealed record GetOwnerReservationsQuery(
    ReservationStatus? Status,
    bool? IsPast,
    int Page = 1,
    int PageSize = 10
) : IRequest<GetOwnerReservationsResult>;