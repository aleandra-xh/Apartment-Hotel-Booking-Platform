
using Booking.Application.Abstractions.Logging;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Booking.Infrastructure.Kafka;

public sealed class KafkaEventLogProducer : IEventLogProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    public KafkaEventLogProducer(IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        _topic = configuration["Kafka:Topic"] ?? "booking-logs";

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync(
        string level,
        string message,
        string? exception,
        Guid? userId,
        string? traceId,
        CancellationToken ct = default)
    {
        var logMessage = new KafkaLogMessage(
            Guid.NewGuid(),
            "Booking.Api",
            level,
            message,
            exception,
            userId,
            traceId,
            DateTime.UtcNow
        );

        var payload = JsonSerializer.Serialize(logMessage);

        await _producer.ProduceAsync(
            _topic,
            new Message<Null, string> { Value = payload },
            ct);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}