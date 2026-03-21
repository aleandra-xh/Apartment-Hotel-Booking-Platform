
using Booking.Application.Abstractions.PropertySeasonalPrices;
using Booking.Domain.PropertySeasonalPrices;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class PropertySeasonalPriceRepository : IPropertySeasonalPriceRepository
{
    private readonly BookingDbContext _dbContext;

    public PropertySeasonalPriceRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasOverlappingSeasonalPriceAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _dbContext.PropertySeasonalPrices
            .AnyAsync(sp =>
                sp.PropertyId == propertyId &&
                start < sp.EndDate.Date &&
                end > sp.StartDate.Date,
                ct);
    }

    public async Task<List<PropertySeasonalPrice>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default)
    {
        return await _dbContext.PropertySeasonalPrices
            .Where(sp => sp.PropertyId == propertyId)
            .OrderBy(sp => sp.StartDate)
            .ToListAsync(ct);
    }

    public async Task<PropertySeasonalPrice?> GetByIdAsync(Guid seasonalPriceId, CancellationToken ct = default)
    {
        return await _dbContext.PropertySeasonalPrices
            .FirstOrDefaultAsync(sp => sp.Id == seasonalPriceId, ct);
    }

    public async Task<decimal?> GetApplicablePricePerNightAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var seasonalPrice = await _dbContext.PropertySeasonalPrices
            .Where(sp =>
                sp.PropertyId == propertyId &&
                start >= sp.StartDate.Date &&
                end <= sp.EndDate.Date)
            .OrderBy(sp => sp.StartDate)
            .FirstOrDefaultAsync(ct);

        return seasonalPrice?.PricePerNight;
    }
}