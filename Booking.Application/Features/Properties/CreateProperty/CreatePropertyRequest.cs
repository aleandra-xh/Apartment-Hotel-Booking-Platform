

namespace Booking.Application.Features.Properties.CreateProperty;

public sealed record CreatePropertyRequest
(
    string Name,
    string Description,
    int PropertyType,
    CreatePropertyAddressDto Address,
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

