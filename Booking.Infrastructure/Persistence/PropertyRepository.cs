using Booking.Application.Abstractions.Properties;
using Booking.Domain.Properties;
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
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _dbContext.Properties
            .Include(p => p.Address)
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