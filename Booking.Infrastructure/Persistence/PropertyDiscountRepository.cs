
using Booking.Application.Abstractions.PropertyDiscounts;
using Booking.Domain.PropertyDiscounts;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class PropertyDiscountRepository : IPropertyDiscountRepository
{
    private readonly BookingDbContext _dbContext;

    public PropertyDiscountRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasOverlappingDiscountAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _dbContext.PropertyDiscounts
            .AnyAsync(d =>
                d.PropertyId == propertyId &&
                start < d.EndDate.Date &&
                end > d.StartDate.Date,
                ct);
    }

    public async Task<List<PropertyDiscount>> GetByPropertyIdAsync(Guid propertyId, CancellationToken ct = default)
    {
        return await _dbContext.PropertyDiscounts
            .Where(d => d.PropertyId == propertyId)
            .OrderBy(d => d.StartDate)
            .ToListAsync(ct);
    }

    public async Task<PropertyDiscount?> GetByIdAsync(Guid discountId, CancellationToken ct = default)
    {
        return await _dbContext.PropertyDiscounts
            .FirstOrDefaultAsync(d => d.Id == discountId, ct);
    }

    public async Task<decimal?> GetApplicableDiscountPercentageAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var discount = await _dbContext.PropertyDiscounts
            .Where(d =>
                d.PropertyId == propertyId &&
                start >= d.StartDate.Date &&
                end <= d.EndDate.Date)
            .OrderBy(d => d.StartDate)
            .FirstOrDefaultAsync(ct);

        return discount?.Percentage;
    }
}