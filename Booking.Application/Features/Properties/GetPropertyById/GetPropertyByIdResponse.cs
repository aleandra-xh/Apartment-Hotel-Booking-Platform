

namespace Booking.Application.Features.Properties.GetPropertyById;

public sealed record GetPropertyByIdResponse(
    Guid Id,
    string Name,
    string Description,
    string PropertyType,
    int MaxGuests,
    decimal PricePerNight,
    decimal CleaningFee,
    decimal ServiceFee,
    decimal TaxPercentage,
    decimal AdditionalGuestFeePerNight,
    int BaseGuestCount,
    int MinStayNights,
    int MaxStayNights,
    string CheckInTime,
    string CheckOutTime,
    bool IsActive,
    bool IsApproved,
    DateTime CreatedAt,
    DateTime? LastModifiedAt,
    double AverageRating,
    int ReviewsCount,
    int ImagesCount,
    GetPropertyAddressResponse Address,
    List<string> Amenities
);
