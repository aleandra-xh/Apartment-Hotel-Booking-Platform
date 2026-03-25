
using Booking.Domain.Users;

namespace Booking.Application.Abstractions.LogIn;

public interface IAuthManager
{
    string GenerateToken(User user);
    int GetExpiresSeconds();
    DateTime GetAccessTokenExpiresAtUtc();
}