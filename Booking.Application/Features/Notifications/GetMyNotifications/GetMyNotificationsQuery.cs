
using MediatR;

namespace Booking.Application.Features.Notifications.GetMyNotifications;

public sealed record GetMyNotificationsQuery(
    int Page,
    int PageSize
) : IRequest<GetMyNotificationsResult>;