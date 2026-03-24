
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Users;

namespace Booking.Application.Abstractions.UserRegister;
public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsEmailUnique(string email, CancellationToken ct);
    Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct);
    Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct);
}