
using Booking.Domain.Users;

namespace Booking.Domain.Notifications;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}