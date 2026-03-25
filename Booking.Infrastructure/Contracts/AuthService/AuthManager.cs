using Booking.Application.Abstractions.LogIn;
using Booking.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Booking.Infrastructure.Contracts.AuthService;

public sealed class AuthManager : IAuthManager
{
    private readonly IConfiguration _config;

    public AuthManager(IConfiguration config)
    {
        _config = config;
    }
    public DateTime GetAccessTokenExpiresAtUtc()
    {
        return DateTime.UtcNow.AddSeconds(GetExpiresSeconds());
    }
    public int GetExpiresSeconds()
    {
        var minutesStr = _config["Jwt:ExpiresMinutes"];
        if (!int.TryParse(minutesStr, out var minutes)) minutes = 60;
        return minutes * 60;
    }

    public string GenerateToken(User user)
    {
        var key = _config["Jwt:SecretKey"];
        if (string.IsNullOrWhiteSpace(key))
            throw new Exception("Jwt:SecretKey is missing.");

        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var expiresSeconds = GetExpiresSeconds();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var roles = user.UserRoles.Select(ur => ur.Role.Name.ToString());
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expiresSeconds),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}