
namespace Booking.Application.Features.Properties.GetPropertyById;

public sealed record GetPropertyAddressResponse(
    string Country,
    string City,
    string Street,
    string PostalCode
);