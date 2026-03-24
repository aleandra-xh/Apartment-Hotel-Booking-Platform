
namespace Booking.Application.Features.Reservations.GetAllReservationsForAdmin;

public sealed record GetAllReservationsForAdminResult(
    List<GetAllReservationsForAdminResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);