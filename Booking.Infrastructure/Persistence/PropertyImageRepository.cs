using System;
using System.Collections.Generic;
using System.Text;

using Booking.Application.Abstractions.PropertyImages;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;
public sealed class PropertyImageRepository : IPropertyImageRepository
{
    private readonly BookingDbContext _dbContext;

    public PropertyImageRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsByHashAsync(Guid propertyId, string imageHash, CancellationToken ct = default)
    {
        return await _dbContext.PropertyImages
            .AnyAsync(pi => pi.PropertyId == propertyId && pi.ImageHash == imageHash, ct);
    }
}