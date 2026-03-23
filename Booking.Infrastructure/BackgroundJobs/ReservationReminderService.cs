
using Booking.Application.Abstractions.Notifications;
using Booking.Domain.Notifications;
using Booking.Domain.Reservations;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Booking.Infrastructure.BackgroundJobs;

public sealed class ReservationReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReservationReminderService(IServiceScopeFactory scopeFactory)
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

            var tomorrow = DateTime.UtcNow.Date.AddDays(1);

            var reservationsToRemind = await dbContext.Reservations
                .Include(r => r.Property)
                .Where(r =>
                    r.BookingStatus == ReservationStatus.Confirmed &&
                    r.StartDate.Date == tomorrow &&
                    !r.ReminderSent)
                .ToListAsync(stoppingToken);

            foreach (var reservation in reservationsToRemind)
            {
                reservation.ReminderSent = true;
                reservation.LastModifiedAt = DateTime.UtcNow;
            }

            if (reservationsToRemind.Count > 0)
            {
                await dbContext.SaveChangesAsync(stoppingToken);

                foreach (var reservation in reservationsToRemind)
                {
                    await notificationService.CreateAsync(
                        reservation.GuestId,
                        "Booking reminder",
                        $"Reminder: your stay at '{reservation.Property.Name}' starts tomorrow.",
                        NotificationType.BookingReminder,
                        stoppingToken);

                    var guest = await dbContext.Set<User>()
                        .FirstOrDefaultAsync(u => u.Id == reservation.GuestId, stoppingToken);

                    if (guest is not null && !string.IsNullOrWhiteSpace(guest.Email))
                    {
                        await emailService.SendAsync(
                            new EmailMessage(
                                guest.Email,
                                "Booking reminder",
                                $"Reminder: your stay at '{reservation.Property.Name}' starts tomorrow."
                            ),
                            stoppingToken);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}