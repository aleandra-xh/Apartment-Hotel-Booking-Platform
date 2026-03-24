
using global::Booking.Application.Features.Notifications.GetMyNotifications;

namespace Booking.Application.Abstractions.Queries;

public interface INotificationQueryService
{
    Task<GetMyNotificationsResult> GetMyNotificationsAsync(Guid userId, int page, int pageSize, CancellationToken ct);
}