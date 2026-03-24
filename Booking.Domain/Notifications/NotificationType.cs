
namespace Booking.Domain.Notifications;

public enum NotificationType
{
    BookingCreated=1,
    BookingConfirmed,
    BookingRejected,
    BookingCancelled,
    BookingReminder,
    BookingCompleted,
    BookingExpired,
    NewReview,
    PropertyApproved,
    PropertyRejected,
    PropertySuspended,
    OwnerRequestApproved,
    OwnerRequestRejected,
    AccountSuspended
}