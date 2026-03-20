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
            .Include(p => p.Reservations)
            .Include(p => p.Amenities)
            .Include(p => p.BlockedDates)
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
    List<int>? amenityIds,
    double? minRating,
    string? sortBy,
    string? sortDirection,
    int page,
    int pageSize,
    CancellationToken ct = default)
    {
        var query = _dbContext.Properties
            .Include(p => p.Address)
            .Include(p => p.Reservations)
            .Include(p => p.Amenities)
            .Include(p => p.BlockedDates)
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

        if (amenityIds is { Count: > 0 })
        {
            query = query.Where(p =>
                amenityIds.All(amenityId =>
                    p.Amenities.Any(pa => (int)pa.Amenity == amenityId)));
        }

        if (minRating.HasValue)
        {
            query = query.Where(p =>
                _dbContext.Reviews.Any(rv => p.Reservations.Select(r => r.Id).Contains(rv.ReservationId)) &&
                (_dbContext.Reviews
                    .Where(rv => p.Reservations.Select(r => r.Id).Contains(rv.ReservationId))
                    .Average(rv => (double?)rv.Rating) ?? 0) >= minRating.Value);
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            var start = startDate.Value.Date;
            var end = endDate.Value.Date;
            var numberOfNights = (end - start).Days;

            query = query.Where(p =>
                numberOfNights >= p.MinStayNights &&
                numberOfNights <= p.MaxStayNights);

            query = query.Where(p => !p.BlockedDates.Any(b =>
                start < b.EndDate.Date &&
                end > b.StartDate.Date
            ));

            query = query.Where(p => !p.Reservations.Any(r =>
                (r.BookingStatus == ReservationStatus.Pending ||
                 r.BookingStatus == ReservationStatus.Confirmed) &&
                start < r.EndDate.Date &&
                end > r.StartDate.Date
            ));
        }

        query = query.Where(p => p.IsActive && p.IsApproved);

        var totalCount = await query.CountAsync(ct);

        IQueryable<Property> orderedQuery;

        var sortByNormalized = sortBy?.Trim().ToLower();
        var sortDirectionNormalized = sortDirection?.Trim().ToLower();

        if (sortByNormalized == "price")
        {
            orderedQuery = sortDirectionNormalized == "desc"
                ? query.OrderByDescending(p => p.PricePerNight)
                : query.OrderBy(p => p.PricePerNight);
        }
        else if (sortByNormalized == "rating")
        {
            orderedQuery = sortDirectionNormalized == "desc"
                ? query.OrderByDescending(p =>
                    _dbContext.Reviews
                        .Where(rv => p.Reservations.Select(r => r.Id).Contains(rv.ReservationId))
                        .Average(rv => (double?)rv.Rating) ?? 0)
                : query.OrderBy(p =>
                    _dbContext.Reviews
                        .Where(rv => p.Reservations.Select(r => r.Id).Contains(rv.ReservationId))
                        .Average(rv => (double?)rv.Rating) ?? 0);
        }
        else
        {
            orderedQuery = sortDirectionNormalized == "desc"
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name);
        }

        var items = await orderedQuery
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