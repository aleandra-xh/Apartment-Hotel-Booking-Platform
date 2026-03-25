
using Booking.Application.Abstractions.LogIn;
using MediatR;

namespace Booking.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public LogoutCommandHandler(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken ct)
    {
        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, ct);
        return Unit.Value;
    }
}