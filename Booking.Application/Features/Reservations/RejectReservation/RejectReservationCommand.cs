
using MediatR;

namespace Booking.Application.Features.Reservations.RejectReservation;

public sealed record RejectReservationCommand(Guid ReservationId) : IRequest<Unit>;