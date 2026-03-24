using Booking.Application.Abstractions.Queries;
using Booking.Application.Abstractions.Security;
using MediatR;

namespace Booking.Application.Features.Notifications.GetMyNotifications;

public sealed class GetMyNotificationsQueryHandler
    : IRequestHandler<GetMyNotificationsQuery, GetMyNotificationsResult>
{
    private readonly INotificationQueryService _notificationQueryService;
    private readonly ICurrentUserService _currentUserService;

    public GetMyNotificationsQueryHandler(
        INotificationQueryService notificationQueryService,
        ICurrentUserService currentUserService)
    {
        _notificationQueryService = notificationQueryService;
        _currentUserService = currentUserService;
    }

    public async Task<GetMyNotificationsResult> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        return await _notificationQueryService.GetMyNotificationsAsync(userId, page, pageSize, ct);
    }
}