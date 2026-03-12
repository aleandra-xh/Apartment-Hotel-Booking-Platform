
using MediatR;

namespace Booking.Application.Features.Reservations.ConfirmReservation;

public sealed record ConfirmReservationCommand(Guid ReservationId) : IRequest<Unit>;
