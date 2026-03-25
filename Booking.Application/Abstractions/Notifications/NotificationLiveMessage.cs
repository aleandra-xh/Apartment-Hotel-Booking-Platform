
using Booking.Domain.Notifications;

namespace Booking.Application.Abstractions.Notifications;

public sealed record NotificationLiveMessage(
    Guid Id,
    string Title,
    string Message,
    NotificationType Type,
    bool IsRead,
    DateTime CreatedAt
);