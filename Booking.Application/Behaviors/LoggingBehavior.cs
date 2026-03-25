using Booking.Application.Abstractions.Logging;
using Booking.Application.Abstractions.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Booking.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IEventLogProducer _eventLogProducer;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        IEventLogProducer eventLogProducer,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _eventLogProducer = eventLogProducer;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var traceId = Activity.Current?.TraceId.ToString();
        Guid? userId = _currentUserService.UserId == Guid.Empty
    ? null
    : _currentUserService.UserId;

        var startMessage = $"Handling request {requestName}";

        _logger.LogInformation("Handling request {RequestName} {@Request}",
            requestName, request);

        await _eventLogProducer.PublishAsync(
            "Information",
            startMessage,
            null,
            userId,
            traceId,
            cancellationToken);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            var finishMessage = $"Finished request {requestName} in {stopwatch.ElapsedMilliseconds}ms";

            _logger.LogInformation(
                "Finished request {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            await _eventLogProducer.PublishAsync(
                "Information",
                finishMessage,
                null,
                userId,
                traceId,
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var errorMessage = $"Request {requestName} failed after {stopwatch.ElapsedMilliseconds}ms";

            _logger.LogError(ex,
                "Request {RequestName} failed after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            await _eventLogProducer.PublishAsync(
                "Error",
                errorMessage,
                ex.ToString(),
                userId,
                traceId,
                cancellationToken);

            throw;
        }
    }
}