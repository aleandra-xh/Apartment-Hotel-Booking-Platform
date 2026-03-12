
using MediatR;

namespace Booking.Application.Features.Reservations.CancelReservation;

public sealed record CancelReservationCommand(Guid ReservationId) : IRequest<Unit>;