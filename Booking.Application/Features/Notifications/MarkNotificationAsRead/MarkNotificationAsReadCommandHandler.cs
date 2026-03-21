
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using MediatR;

namespace Booking.Application.Features.Notifications.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Unit>
{
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkNotificationAsReadCommandHandler(
        IGenericRepository<Notification> notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var notification = await _notificationRepository.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId,
            ct);

        if (notification is null)
            throw new NotFoundException("Notification not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedException("You are not allowed to modify this notification.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}