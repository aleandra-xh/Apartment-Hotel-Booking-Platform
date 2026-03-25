
namespace Booking.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenResponse(
    string Token,
    string RefreshToken,
    string Type,
    int Expiration
);