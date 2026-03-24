
using Booking.Domain.Reservations;
using MediatR;

namespace Booking.Application.Features.Reservations.GetAllReservationsForAdmin;

public sealed record GetAllReservationsForAdminQuery(
    ReservationStatus? Status,
    int Page,
    int PageSize
) : IRequest<GetAllReservationsForAdminResult>;