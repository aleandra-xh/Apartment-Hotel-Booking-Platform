
namespace Booking.Application.Abstractions.Logging;

public interface IEventLogProducer
{
    Task PublishAsync(
        string level,
        string message,
        string? exception,
        Guid? userId,
        string? traceId,
        CancellationToken ct = default);
}
