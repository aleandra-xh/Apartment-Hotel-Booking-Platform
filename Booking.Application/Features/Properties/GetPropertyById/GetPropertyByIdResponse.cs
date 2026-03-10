

namespace Booking.Application.Features.Properties.GetPropertyById;

public sealed record GetPropertyByIdResponse
    (
    Guid Id,
    string Name,
    string Description,
    string PropertyType,
    int MaxGuests,
    string CheckInTime,
    string CheckOutTime,
    bool IsActive,
    bool IsApproved,
    GetPropertyAddressResponse Address,
    List<string> Amenities
);
