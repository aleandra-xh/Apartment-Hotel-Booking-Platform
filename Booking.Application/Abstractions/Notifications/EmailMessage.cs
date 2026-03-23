
namespace Booking.Application.Abstractions.Notifications;

public sealed record EmailMessage(
    string ToEmail,
    string Subject,
    string PlainTextContent,
    string? HtmlContent = null
);