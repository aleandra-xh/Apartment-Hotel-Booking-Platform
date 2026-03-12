
using MediatR;

namespace Booking.Application.Features.Reservations.CompleteReservation;

public sealed record CompleteReservationCommand(Guid ReservationId) : IRequest<Unit>;