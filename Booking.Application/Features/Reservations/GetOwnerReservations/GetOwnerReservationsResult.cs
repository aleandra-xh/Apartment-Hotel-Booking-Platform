
namespace Booking.Application.Features.Reservations.GetOwnerReservations;

public sealed record GetOwnerReservationsResult(
    List<GetOwnerReservationsResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);