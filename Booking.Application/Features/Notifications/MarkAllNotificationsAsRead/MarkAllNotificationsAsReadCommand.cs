
using MediatR;

namespace Booking.Application.Features.Notifications.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand : IRequest<Unit>;