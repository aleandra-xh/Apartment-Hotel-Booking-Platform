using Booking.Application.Abstractions.Properties;
using Booking.Domain.Properties;
using Booking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly BookingDbContext _dbContext;

    public PropertyRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Property?> GetPropertyByIdWithDetailsAsync(Guid propertyId, CancellationToken ct = default)
    {
        return await _dbContext.Properties
            .Include(p => p.Address)
            .FirstOrDefaultAsync(p => p.Id == propertyId, ct);
    }

    public async Task<(List<Property> Items, int TotalCount)> SearchPropertiesAsync(
        string? city,
        int? maxGuests,
        int? propertyType,
        DateTime? startDate,
        DateTime? endDate,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _dbContext.Properties
            .Include(p => p.Address)
            .Include(p => p.Reservations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalizedCity = city.Trim().ToLower();
            query = query.Where(p => p.Address.City.ToLower() == normalizedCity);
        }

        if (maxGuests.HasValue)
        {
            query = query.Where(p => p.MaxGuests >= maxGuests.Value);
        }

        if (propertyType.HasValue)
        {
            query = query.Where(p => (int)p.PropertyType == propertyType.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.PricePerNight >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.PricePerNight <= maxPrice.Value);
        }

        Console.WriteLine($"Repo StartDate: {startDate}");
        Console.WriteLine($"Repo EndDate: {endDate}");

        if (startDate.HasValue && endDate.HasValue)
        {
            var start = startDate.Value.Date;
            var end = endDate.Value.Date;

            query = query.Where(p => !p.Reservations.Any(r =>
                (r.BookingStatus == ReservationStatus.Pending ||
                 r.BookingStatus == ReservationStatus.Confirmed) &&
                start < r.EndDate.Date &&
                end > r.StartDate.Date
            ));
        }

        query = query.Where(p => p.IsActive && p.IsApproved);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Property?> GetPropertyForReservationAsync(Guid propertyId, CancellationToken ct = default)
    {
        return await _dbContext.Properties
            .FirstOrDefaultAsync(p => p.Id == propertyId, ct);
    }
}