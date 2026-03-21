
using Booking.Domain.Notifications;

namespace Booking.Application.Features.Notifications.GetMyNotifications;

public sealed record GetMyNotificationsResponse(
    Guid Id,
    string Title,
    string Message,
    NotificationType Type,
    bool IsRead,
    DateTime CreatedAt
);