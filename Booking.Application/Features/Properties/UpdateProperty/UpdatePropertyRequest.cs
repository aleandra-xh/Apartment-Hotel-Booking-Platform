
namespace Booking.Application.Features.Properties.UpdateProperty;

public sealed record UpdatePropertyRequest
(
    string Name,
    string Description,
    int PropertyType,
    int MaxGuests,
    string CheckInTime,
    string CheckOutTime,
    List<int> Amenities
);
