
using Booking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Booking.Infrastructure.BackgroundJobs;

public sealed class ReservationExpirationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReservationExpirationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            var expirationThreshold = DateTime.UtcNow.AddHours(-24);

            var reservationsToExpire = await dbContext.Reservations
                .Where(r =>
                    r.BookingStatus == ReservationStatus.Pending &&
                    r.CreatedAt < expirationThreshold)
                .ToListAsync(stoppingToken);

            foreach (var reservation in reservationsToExpire)
            {
                reservation.BookingStatus = ReservationStatus.Expired;
                reservation.LastModifiedAt = DateTime.UtcNow;
            }

            if (reservationsToExpire.Count > 0)
            {
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}