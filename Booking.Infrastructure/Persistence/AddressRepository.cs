
using Booking.Application.Abstractions.Addresses;
using Booking.Domain.Addresses;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class AddressRepository : IAddressRepository
{
    private readonly BookingDbContext _dbContext;

    public AddressRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Address?> GetExistingAddressAsync(
        string country,
        string city,
        string street,
        string postalCode,
        CancellationToken ct = default)
    {
        var normalizedCountry = country.Trim().ToLower();
        var normalizedCity = city.Trim().ToLower();
        var normalizedStreet = street.Trim().ToLower();
        var normalizedPostalCode = postalCode.Trim().ToLower();

        return await _dbContext.Addresses
            .FirstOrDefaultAsync(a =>
                a.Country.ToLower() == normalizedCountry &&
                a.City.ToLower() == normalizedCity &&
                a.Street.ToLower() == normalizedStreet &&
                a.PostalCode.ToLower() == normalizedPostalCode,
                ct);
    }
}