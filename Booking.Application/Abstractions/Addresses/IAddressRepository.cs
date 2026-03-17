
using Booking.Domain.Addresses;

namespace Booking.Application.Abstractions.Addresses;

public interface IAddressRepository
{
    Task<Address?> GetExistingAddressAsync(
        string country,
        string city,
        string street,
        string postalCode,
        CancellationToken ct = default);
}