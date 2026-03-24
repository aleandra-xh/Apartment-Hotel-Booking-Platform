
using Booking.Application.Abstractions.Security;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using MediatR;

namespace Booking.Application.Features.Notifications.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsAsReadCommand, Unit>
{
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkAllNotificationsAsReadCommandHandler(
        IGenericRepository<Notification> notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var notifications = await _notificationRepository.GetAllAsync(
            n => n.UserId == userId && !n.IsRead,
            ct);

        if (notifications.Count == 0)
            return Unit.Value;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _notificationRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}