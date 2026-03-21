
using Booking.Application.Abstractions.Notifications;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;


namespace Booking.Infrastructure.Notifications;

public sealed class NotificationService : INotificationService
{
    private readonly IGenericRepository<Notification> _notificationRepository;

    public NotificationService(IGenericRepository<Notification> notificationRepository)
    {
        _notificationRepository = notificationRepository;
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
    }
}
