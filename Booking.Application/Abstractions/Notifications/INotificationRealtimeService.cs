
namespace Booking.Application.Abstractions.Notifications;

public interface INotificationRealtimeService
{
    Task SendToUserAsync(
        Guid userId,
        NotificationLiveMessage notification,
        CancellationToken ct = default);
}
