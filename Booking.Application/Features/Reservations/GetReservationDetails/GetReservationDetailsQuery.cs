
using MediatR;

namespace Booking.Application.Features.Reservations.GetReservationDetails;
public sealed record GetReservationDetailsQuery(Guid ReservationId)
    : IRequest<GetReservationDetailsResponse>;