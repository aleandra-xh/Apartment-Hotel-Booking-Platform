
using Booking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Booking.Infrastructure.BackgroundJobs;

public sealed class ReservationCompletionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReservationCompletionService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            var today = DateTime.UtcNow.Date;

            var reservationsToComplete = await dbContext.Reservations
                .Where(r =>
                    r.BookingStatus == ReservationStatus.Confirmed &&
                    r.EndDate.Date < today)
                .ToListAsync(stoppingToken);

            foreach (var reservation in reservationsToComplete)
            {
                reservation.BookingStatus = ReservationStatus.Completed;
                reservation.CompletedOnUtc = DateTime.UtcNow;
                reservation.LastModifiedAt = DateTime.UtcNow;
            }

            if (reservationsToComplete.Count > 0)
            {
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}