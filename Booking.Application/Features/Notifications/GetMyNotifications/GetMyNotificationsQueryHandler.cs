
using Booking.Application.Abstractions.Security;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Notifications;
using MediatR;

namespace Booking.Application.Features.Notifications.GetMyNotifications;

public sealed class GetMyNotificationsQueryHandler
    : IRequestHandler<GetMyNotificationsQuery, GetMyNotificationsResult>
{
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyNotificationsQueryHandler(
        IGenericRepository<Notification> notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetMyNotificationsResult> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var notifications = await _notificationRepository.GetAllAsync(
            n => n.UserId == userId,
            ct);

        var ordered = notifications
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = ordered.Count();

        var items = ordered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new GetMyNotificationsResponse(
                n.Id,
                n.Title,
                n.Message,
                n.Type,
                n.IsRead,
                n.CreatedAt
            ))
            .ToList();

        return new GetMyNotificationsResult(
            items,
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}