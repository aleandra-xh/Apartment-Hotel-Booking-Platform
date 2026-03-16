
namespace Booking.Application.Features.Properties.UpdateProperty;

public sealed record UpdatePropertyRequest
(
    string Name,
    string Description,
    int PropertyType,
    int MaxGuests,
    decimal PricePerNight,
    decimal CleaningFee,
    decimal ServiceFee,
    decimal TaxPercentage,
    decimal AdditionalGuestFeePerNight,
    int BaseGuestCount,
    string CheckInTime,
    string CheckOutTime,
    List<int> Amenities
);
