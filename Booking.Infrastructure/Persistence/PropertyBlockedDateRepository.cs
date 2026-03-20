
using Booking.Application.Abstractions.PropertyBlockedDates;
using Booking.Domain.PropertyBlockedDates;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class PropertyBlockedDateRepository : IPropertyBlockedDateRepository
{
    private readonly BookingDbContext _dbContext;

    public PropertyBlockedDateRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasOverlappingBlockedDateAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _dbContext.PropertyBlockedDates
            .AnyAsync(b =>
                b.PropertyId == propertyId &&
                start < b.EndDate.Date &&
                end > b.StartDate.Date,
                ct);
    }

    public async Task<List<PropertyBlockedDate>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default)
    {
        return await _dbContext.PropertyBlockedDates
            .Where(b => b.PropertyId == propertyId)
            .OrderBy(b => b.StartDate)
            .ToListAsync(ct);
    }

    public async Task<PropertyBlockedDate?> GetByIdAsync(Guid blockedDateId, CancellationToken ct = default)
    {
        return await _dbContext.PropertyBlockedDates
            .FirstOrDefaultAsync(b => b.Id == blockedDateId, ct);
    }
}