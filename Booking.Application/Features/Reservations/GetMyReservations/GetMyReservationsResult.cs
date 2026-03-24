
namespace Booking.Application.Features.Reservations.GetMyReservations;

public sealed record GetMyReservationsResult(
    List<GetMyReservationsResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
