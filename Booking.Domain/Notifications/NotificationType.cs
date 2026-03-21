
namespace Booking.Domain.Notifications;

public enum NotificationType
{
    BookingConfirmed = 1,
    BookingCancelled,
    BookingReminder,
    NewReview,
    PropertyApproved,
    PropertyRejected,
    BookingRejected,
    BookingCompleted,
    BookingExpired
}