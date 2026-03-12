using Booking.Domain.Properties;

namespace Booking.Domain.Addresses;

public class Address
{
    public Guid Id { get; set; }

    public string Country { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string PostalCode { get; set; } = null!;

    public List<Property> Properties { get; set; } = new();


}