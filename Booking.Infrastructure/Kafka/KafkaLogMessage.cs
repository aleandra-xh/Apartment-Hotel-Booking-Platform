
namespace Booking.Infrastructure.Kafka;

public sealed record KafkaLogMessage(
    Guid ExternalId,
    string Service,
    string Level,
    string Message,
    string? Exception,
    Guid? UserId,
    string? TraceId,
    DateTime CreatedAtUtc
);