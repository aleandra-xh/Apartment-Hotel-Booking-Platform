
using Booking.Application.Abstractions.Notifications;
using Booking.Domain.Notifications;
using Booking.Domain.Reservations;
using Booking.Domain.Users;
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
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var today = DateTime.UtcNow.Date;

            var reservationsToComplete = await dbContext.Reservations
                .Include(r => r.Property)
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

                foreach (var reservation in reservationsToComplete)
                {
                    await notificationService.CreateAsync(
                        reservation.GuestId,
                        "Booking completed",
                        $"Your stay for property '{reservation.Property.Name}' has been completed. You can now leave a review.",
                        NotificationType.BookingCompleted,
                        stoppingToken);

                    var guest = await dbContext.Set<User>()
                        .FirstOrDefaultAsync(u => u.Id == reservation.GuestId, stoppingToken);

                    if (guest is not null && !string.IsNullOrWhiteSpace(guest.Email))
                    {
                        await emailService.SendAsync(
                            new EmailMessage(
                                guest.Email,
                                "Booking completed",
                                $"Your stay for property '{reservation.Property.Name}' has been completed. You can now leave a review."
                            ),
                            stoppingToken);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}