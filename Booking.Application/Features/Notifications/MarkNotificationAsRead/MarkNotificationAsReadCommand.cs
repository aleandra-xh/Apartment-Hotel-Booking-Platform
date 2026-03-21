
using MediatR;

namespace Booking.Application.Features.Notifications.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<Unit>;