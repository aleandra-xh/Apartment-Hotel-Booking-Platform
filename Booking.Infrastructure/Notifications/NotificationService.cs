
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;

namespace Booking.Infrastructure.Notifications;

public sealed class NotificationService : INotificationService
{
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly INotificationRealtimeService _notificationRealtimeService;

    public NotificationService(
        IGenericRepository<Notification> notificationRepository,
        INotificationRealtimeService notificationRealtimeService)
    {
        _notificationRepository = notificationRepository;
        _notificationRealtimeService = notificationRealtimeService;
    }

    public async Task CreateAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        CancellationToken ct = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, ct);
        await _notificationRepository.SaveChangesAsync(ct);

        await _notificationRealtimeService.SendToUserAsync(
            userId,
            new NotificationLiveMessage(
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.IsRead,
                notification.CreatedAt
            ),
            ct);
    }
}