using MediatR;

namespace Booking.Application.Features.Reservations.GetMyReservations;

public sealed record GetMyReservationsQuery : IRequest<List<GetMyReservationsResponse>>;
