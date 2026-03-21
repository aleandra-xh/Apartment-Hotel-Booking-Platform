
using Booking.Domain.Notifications;

namespace Booking.Application.Abstractions.Notifications;

public interface INotificationService
{
    Task CreateAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        CancellationToken ct = default);
}