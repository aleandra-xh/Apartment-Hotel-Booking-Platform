
namespace Booking.Application.Abstractions.Notifications;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}