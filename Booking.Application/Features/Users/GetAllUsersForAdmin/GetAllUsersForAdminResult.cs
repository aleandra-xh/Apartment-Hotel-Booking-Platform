namespace Booking.Application.Features.Users.GetAllUsersForAdmin;

public sealed record GetAllUsersForAdminResult(
    List<GetAllUsersForAdminResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);