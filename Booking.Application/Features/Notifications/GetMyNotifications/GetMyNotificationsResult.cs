
namespace Booking.Application.Features.Notifications.GetMyNotifications;

public sealed record GetMyNotificationsResult(
    List<GetMyNotificationsResponse> Items,
    int TotalCount,
    int Page,
    int PageSize
);