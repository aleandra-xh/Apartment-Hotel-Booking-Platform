
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
}