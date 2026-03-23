
using Booking.Application.Abstractions.Notifications;
using Booking.Domain.Notifications;
using Booking.Domain.Reservations;
using Booking.Domain.Users;
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
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var expirationThreshold = DateTime.UtcNow.AddHours(-24);

            var reservationsToExpire = await dbContext.Reservations
                .Include(r => r.Property)
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

                foreach (var reservation in reservationsToExpire)
                {
                    await notificationService.CreateAsync(
                        reservation.GuestId,
                        "Booking expired",
                        $"Your booking request for property '{reservation.Property.Name}' has expired.",
                        NotificationType.BookingExpired,
                        stoppingToken);

                    var guest = await dbContext.Set<User>()
                        .FirstOrDefaultAsync(u => u.Id == reservation.GuestId, stoppingToken);

                    if (guest is not null && !string.IsNullOrWhiteSpace(guest.Email))
                    {
                        await emailService.SendAsync(
                            new EmailMessage(
                                guest.Email,
                                "Booking expired",
                                $"Your booking request for property '{reservation.Property.Name}' has expired."
                            ),
                            stoppingToken);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
