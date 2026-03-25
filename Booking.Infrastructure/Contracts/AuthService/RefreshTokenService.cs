
using Booking.Application.Abstractions.LogIn;
using Booking.Domain.RefreshTokens;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Booking.Infrastructure.Contracts.AuthService;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly BookingDbContext _context;

    public RefreshTokenService(BookingDbContext context)
    {
        _context = context;
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public async Task SaveRefreshTokenAsync(
        User user,
        string refreshToken,
        DateTime expiresAtUtc,
        CancellationToken ct)
    {
        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAtUtc = expiresAtUtc,
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _context.RefreshTokens.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<User?> GetUserByValidRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct)
    {
        var entity = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt =>
                rt.Token == refreshToken &&
                !rt.IsRevoked &&
                rt.ExpiresAtUtc > DateTime.UtcNow,
                ct);

        return entity?.User;
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct)
    {
        var entity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);

        if (entity is null || entity.IsRevoked)
            return;

        entity.IsRevoked = true;
        entity.RevokedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
    }
}