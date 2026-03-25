
using Booking.API.Hubs;
using Booking.Application.Abstractions.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Booking.API.Realtime;


























public sealed class NotificationRealtimeService : INotificationRealtimeService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationRealtimeService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(
        Guid userId,
        NotificationLiveMessage notification,
        CancellationToken ct = default)
    {
        await _hubContext
            .Clients
            .User(userId.ToString())
            .SendAsync("notificationReceived", notification, ct);
    }
}
