using Booking.Application.Abstractions.UserRegister;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly BookingDbContext _context;

    public UserRepository(BookingDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsEmailUnique(string email, CancellationToken ct)
    {
        var normalizedEmail = email.Trim().ToLower();

        return !await _context.Users
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail, ct);
    }

    public Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct)
    {
        var normalizedEmail = email.Trim().ToLower();

        return _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == normalizedEmail, ct);
    }

    public Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct)
    {
        return _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }
}