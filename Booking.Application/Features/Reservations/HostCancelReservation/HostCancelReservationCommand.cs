
using MediatR;

namespace Booking.Application.Features.Reservations.HostCancelReservation;

public sealed record HostCancelReservationCommand(Guid ReservationId) : IRequest<Unit>;