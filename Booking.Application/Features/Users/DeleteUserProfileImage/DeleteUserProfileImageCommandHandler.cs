
using Booking.Application.Abstractions.Security;
using Booking.Application.Common.Exceptions;
using Booking.Application.Abstractions.UserRegister;
using Booking.Application.Features.Users.DeleteUserProfileImage;
using MediatR;

namespace Booking.Application.Features.Users;
public sealed class DeleteUserProfileImageCommandHandler : IRequestHandler<DeleteUserProfileImageCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserProfileImageCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteUserProfileImageCommand request, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (string.IsNullOrWhiteSpace(user.ProfileImageUrl))
            throw new NotFoundException("Profile image not found.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-images");
        var fileName = Path.GetFileName(user.ProfileImageUrl);
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        user.ProfileImageUrl = null;

        await _userRepository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}