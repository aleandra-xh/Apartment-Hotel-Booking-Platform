
using Booking.Application.Abstractions.Notifications;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Booking.Infrastructure.Notifications;

public sealed class SendGridEmailService : IEmailService
{
    private readonly SendGridSettings _settings;

    public SendGridEmailService(IOptions<SendGridSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return;

        var client = new SendGridClient(_settings.ApiKey);

        var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
        var to = new EmailAddress(message.ToEmail);

        var mail = MailHelper.CreateSingleEmail(
            from,
            to,
            message.Subject,
            message.PlainTextContent,
            message.HtmlContent ?? message.PlainTextContent);

        await client.SendEmailAsync(mail, ct);
    }
}