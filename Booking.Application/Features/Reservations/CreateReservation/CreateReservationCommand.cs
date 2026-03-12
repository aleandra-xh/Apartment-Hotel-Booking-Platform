
using MediatR;

namespace Booking.Application.Features.Reservations.CreateReservation;
public sealed record CreateReservationCommand(CreateReservationRequest Request): IRequest<Guid>;