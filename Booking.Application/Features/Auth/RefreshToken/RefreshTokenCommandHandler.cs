
using Booking.Application.Abstractions.LogIn;
using Booking.Application.Common.Exceptions;
using MediatR;

namespace Booking.Application.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAuthManager _authManager;

    public RefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService,
        IAuthManager authManager)
    {
        _refreshTokenService = refreshTokenService;
        _authManager = authManager;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var user = await _refreshTokenService.GetUserByValidRefreshTokenAsync(
            request.RefreshToken,
            ct);

        if (user is null)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        if (user.IsDeleted)
            throw new UnauthorizedException("This account has been deleted.");

        if (!user.IsActive)
            throw new UnauthorizedException("Your account has been suspended.");

        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, ct);

        var newAccessToken = _authManager.GenerateToken(user);
        var newRefreshToken = _refreshTokenService.GenerateRefreshToken();
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        await _refreshTokenService.SaveRefreshTokenAsync(
            user,
            newRefreshToken,
            refreshTokenExpiresAtUtc,
            ct);

        return new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken,
            "Bearer",
            _authManager.GetExpiresSeconds()
        );
    }
}
