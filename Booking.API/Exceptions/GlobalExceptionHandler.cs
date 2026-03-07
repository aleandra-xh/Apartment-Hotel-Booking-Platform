using Booking.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception,
            "Unhandled exception. Path: {Path}, User: {User}",
            httpContext.Request.Path,
            httpContext.User?.Identity?.Name ?? "anonymous");

        var details = GetExceptionDetails(exception);

        var problem = new ProblemDetails
        {
            Status = details.Status,
            Type = details.Type,
            Title = details.Title,
            Detail = details.Detail,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (exception is ValidationException ve)
        {
            problem.Extensions["errors"] = ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = details.Status;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }

    private static ExceptionDetails GetExceptionDetails(Exception ex)
    {
        var _ = new ExceptionDetails(
            StatusCodes.Status500InternalServerError,
            "https://httpstatuses.com/500",
            "Server error",
            "Ndodhi nje gabim i papritur."
        );

        return ex switch
        {
            ValidationException ve => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "https://httpstatuses.com/400",
                "Validation failed",
                 "One or more validation errors occurred."
            ),

            UnauthorizedAccessException uae => new ExceptionDetails(
                StatusCodes.Status401Unauthorized,
                "https://httpstatuses.com/401",
                "Unauthorized",
                uae.Message
            ),

            UnauthorizedException ue => new ExceptionDetails(
                StatusCodes.Status401Unauthorized,
                "https://httpstatuses.com/401",
                "Unauthorized",
                ue.Message
            ),

            NotFoundException nfe => new ExceptionDetails(
                StatusCodes.Status404NotFound,
                "https://httpstatuses.com/404",
                "Not found",
                nfe.Message
            ),

            ConflictException ce => new ExceptionDetails(
                StatusCodes.Status409Conflict,
                "https://httpstatuses.com/409",
                "Conflict",
                ce.Message
            ),

            _ => _
        };
    }
}