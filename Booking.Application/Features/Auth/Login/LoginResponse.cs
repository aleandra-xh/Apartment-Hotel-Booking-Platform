
namespace Booking.Application.Features.Auth.Login;

public sealed record LoginResponse(
    string Token,
    string RefreshToken,
    string Type,
    int Expiration
);