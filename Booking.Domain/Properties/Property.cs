
using Booking.Domain.Addresses;
using Booking.Domain.Reservations;
using Booking.Domain.Users;
using Booking.Domain.PropertyAmenities;
using Booking.Domain.PropertyImages;

namespace Booking.Domain.Properties;


public class Property
{

    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public PropertyType PropertyType { get; set; }

    public Guid AddressId { get; set; }
    public Address Address { get; set; } = null!;

    public int MaxGuests { get; set; }

    public decimal PricePerNight { get; set; }
    public decimal CleaningFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal AdditionalGuestFeePerNight { get; set; }
    public int BaseGuestCount { get; set; }

    public TimeSpan CheckInTime { get; set; }
    public TimeSpan CheckOutTime { get; set; }

    public bool IsActive { get; set; }
    public bool IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }

    public DateTime? LastBookedOnUtc { get; set; }

    public List<Reservation> Reservations { get; set; } = new();
    public List<PropertyAmenity> Amenities { get; set; } = new();
    public List<PropertyImage> Images { get; set; } = new();

}
