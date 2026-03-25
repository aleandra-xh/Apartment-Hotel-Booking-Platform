
using Booking.Domain.Users;

namespace Booking.Application.Abstractions.LogIn;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();

    Task SaveRefreshTokenAsync(
        User user,
        string refreshToken,
        DateTime expiresAtUtc,
        CancellationToken ct);

    Task<User?> GetUserByValidRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct);

    Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct);
}