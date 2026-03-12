
namespace Booking.Domain.Reservations
{
    public enum ReservationStatus
    {
        Pending = 1,
        Confirmed,
        Rejected,
        Cancelled,
        Completed,
        Expired
    }
}
