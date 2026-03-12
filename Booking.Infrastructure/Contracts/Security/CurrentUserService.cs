using Booking.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Booking.Infrastructure.Contracts.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value
                ?? _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst("sub")?
                    .Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (!Guid.TryParse(userId, out var guid))
                throw new UnauthorizedAccessException("Invalid user identifier in token.");

            return guid;
        }
    }
}