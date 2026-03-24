
using Booking.Application.Common.Exceptions;
using Booking.Application.Generics.Interfaces;
using Booking.Domain.Users;
using MediatR;

namespace Booking.Application.Features.Users.DeleteUser;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IGenericRepository<User> _userRepository;

    public DeleteUserCommandHandler(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.IsDeleted)
            throw new ConflictException("User is already deleted.");

        user.IsActive = false;
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.LastModifiedAt = DateTime.UtcNow;

        user.FirstName = "Deleted";
        user.LastName = "User";
        user.Email = $"deleted_{user.Id}@deleted.local";
        user.PhoneNumber = "DELETED";
        user.ProfileImageUrl = null;

        await _userRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}