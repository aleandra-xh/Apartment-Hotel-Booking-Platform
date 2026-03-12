
using Booking.Application.Abstractions.Reservations;
using Booking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly BookingDbContext _dbContext;

    public ReservationRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasOverlappingReservationAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct = default)
    {
        return await _dbContext.Reservations.AnyAsync(r =>
            r.PropertyId == propertyId &&
            r.BookingStatus != Domain.Reservations.ReservationStatus.Cancelled &&
            r.BookingStatus != Domain.Reservations.ReservationStatus.Rejected &&
            r.BookingStatus != ReservationStatus.Expired &&
            r.StartDate < endDate &&
            r.EndDate > startDate,
            ct);
    }
}
