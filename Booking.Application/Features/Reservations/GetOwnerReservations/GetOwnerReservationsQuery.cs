
using MediatR;

namespace Booking.Application.Features.Reservations.GetOwnerReservations;
public sealed record GetOwnerReservationsQuery : IRequest<List<GetOwnerReservationsResponse>>;