
using Booking.Application.Abstractions.Queries;
using Booking.Application.Features.Notifications.GetMyNotifications;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Queries;

public sealed class NotificationQueryService : INotificationQueryService
{
    private readonly BookingDbContext _context;

    public NotificationQueryService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<GetMyNotificationsResult> GetMyNotificationsAsync(Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new GetMyNotificationsResponse(
                n.Id,
                n.Title,
                n.Message,
                n.Type,
                n.IsRead,
                n.CreatedAt
            ))
            .ToListAsync(ct);

        return new GetMyNotificationsResult(
            items,
            totalCount,
            page,
            pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)
        );
    }
}