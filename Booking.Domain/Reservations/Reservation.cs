using Booking.Domain.Properties;
using Booking.Domain.Users;
using Booking.Domain.Reviews;

namespace Booking.Domain.Reservations;

public class Reservation
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public Guid GuestId { get; set; }
    public User Guest { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int GuestCount { get; set; }

    public decimal CleaningFee { get; set; }
    public decimal AdditionalGuestFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal AmenitiesUpCharge { get; set; }
    public decimal PriceForPeriod { get; set; }
    public decimal TotalPrice { get; set; }

    public ReservationStatus BookingStatus { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }

    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ConfirmedOnUtc { get; set; }
    public DateTime? RejectedOnUtc { get; set; }
    public DateTime? CompletedOnUtc { get; set; }
    public DateTime? CancelledOnUtc { get; set; }

public Review? Review { get; set; }

}